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
	}

	private void AddressBar_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			Navigate();
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
		string uri = args.Uri;
		if (uri.StartsWith("http://"))
		{
			var parsedUri = new Uri(uri);
			var upgradedUri = parsedUri.UpgradeToHttps();
			webView.Source = upgradedUri;
			args.Cancel = true;
		}
		else
		{
			addressBar.Text = uri;
		}
	}

	private void ButtonGo_Click(object sender, RoutedEventArgs e)
	{
		Navigate();
	}

	private void Navigate()
	{
		if (webView != null && webView.CoreWebView2 != null)
		{
			webView.CoreWebView2.Navigate(addressBar.Text);
		}
	}
}
