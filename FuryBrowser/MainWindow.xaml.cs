using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;

namespace FuryBrowser;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
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
		this.addressBar.KeyDown += AddressBar_KeyDown;
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

	private void AddressBar_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			Navigate(addressBar.Text);
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
		// even if for a fleeting moment...
		addressBar.Text = args.Uri;
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

	private void ButtonGo_Click(object sender, RoutedEventArgs e)
	{
		Navigate(addressBar.Text);
	}

	private void Navigate(string text)
	{
		if (webView != null && webView.CoreWebView2 != null)
		{
			var uri = InterpretOmnibarText(text);
			webView.CoreWebView2.Navigate(uri?.ToString() ?? text);
		}
	}
}
