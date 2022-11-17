using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;

namespace FuryBrowser;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : Window
{
	public MainView()
	{
		// TODO: read from the registry and/or settings
		TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
		InitializeComponent();
		webView.NavigationStarting += EnsureHttps;
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

	async void Navigate(string text)
	{
		await webView.EnsureCoreWebView2Async();
		var uri = InterpretOmnibarText(text);
		webView.CoreWebView2.Navigate(uri?.ToString() ?? text);
	}

	void GoToPageCmdExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		Navigate((string)e.Parameter);
	}

	private void GoToPageCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = webView != null /* && !isNavigating */;
	}
}
