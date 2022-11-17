using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using ReactiveUI;

namespace FuryBrowser;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView
{
	private static readonly Uri DefaultUri = new("https://duckduckgo.com");

	// TODO: This should probably be an observable property
	public Uri? Source
	{
		get => webView.Source;
		set
		{
			webView.EnsureCoreWebView2Async();
			var destination = value ?? DefaultUri;
			if (webView.CoreWebView2 != null)
			{
				webView.CoreWebView2.Navigate(destination.ToString());
			}
			else
			{
				webView.Source = destination;
			}
		}
	}

	public MainView()
	{
		// TODO: read from the registry and/or settings
		TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
		InitializeComponent();
		ViewModel = new MainViewModel();

		webView.NavigationStarting += EnsureHttps;

		this.WhenActivated(disposableRegistration =>
		{
			this.OneWayBind(ViewModel,
				viewModel => viewModel.Destination,
				view => view.Source)
				.DisposeWith(disposableRegistration);

			// We want the address bar to update based on the WebView's Source,
			// but we don't want the WebView to navigate just from the user typing
			// into the address bar. Therefore we use WhenAnyValue to listen to Source
			this.WhenAnyValue(v => v.webView.Source)
				.Select(u => u.ToString())
				.BindTo(this, x => x.ViewModel!.OmnibarText)
				.DisposeWith(disposableRegistration);
			this.Bind(ViewModel,
				viewModel => viewModel.OmnibarText,
				view => view.addressBar.Text)
				.DisposeWith(disposableRegistration);

			this.BindCommand(ViewModel,
				viewModel => viewModel.GoToPage,
				view => view.ButtonGo,
				viewModel => viewModel.OmnibarText)
				.DisposeWith(disposableRegistration);

			var enterGoes = new KeyBinding(
				ViewModel.GoToPage,
				Key.Return,
				ModifierKeys.None
			);
			addressBar.InputBindings.Add(enterGoes);
		});
		var goToAddressBar = new GoToAddressBar(addressBar);
		this.InputBindings.Add(new KeyBinding(
			goToAddressBar,
			Key.D,
			ModifierKeys.Alt
		));
		this.InputBindings.Add(new KeyBinding(
			goToAddressBar,
			Key.L,
			ModifierKeys.Control
		));
		this.addressBar.GotFocus += AddressBar_GotFocus;
		this.addressBar.LostFocus += AddressBar_LostFocus;
	}

	private void AddressBar_LostFocus(object sender, RoutedEventArgs e)
	{
		addressBar.Select(0, 0);
	}

	private void AddressBar_GotFocus(object sender, RoutedEventArgs e)
	{
		if (addressBar == e.Source)
		{
			addressBar.SelectAll();
		}
	}

	class GoToAddressBar : ICommand
	{
		private readonly TextBox _destination;

		public GoToAddressBar(TextBox destination)
		{
			_destination = destination;
		}

		public bool CanExecute(object? parameter)
		{
			return true;
		}

		public void Execute(object? parameter)
		{
			_destination.Focus();
		}

		public event EventHandler? CanExecuteChanged;
	}

	private void EnsureHttps(object? sender, CoreWebView2NavigationStartingEventArgs args)
	{
		var uri = InterpretOmnibarText(args.Uri);
		if (uri != null)
		{
			webView.Source = uri;
			args.Cancel = true;
		}
	}

	internal static Uri? InterpretOmnibarText(string s)
	{
		if (s.StartsWith("http://"))
		{
			var parsedUri = new Uri(s);
			var upgradedUri = parsedUri.UpgradeToHttps();
			return upgradedUri;
		}
		if (s.StartsWith("https://") || s.StartsWith("about:") || s.StartsWith("edge:"))
		{
			return null;
		}

		var finalDestination = UriExtensions.GuessDestination(s);
		return finalDestination;
	}
}
