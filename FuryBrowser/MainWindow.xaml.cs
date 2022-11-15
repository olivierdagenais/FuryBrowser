using System.Windows;
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
	}

	private void EnsureHttps(object? sender, CoreWebView2NavigationStartingEventArgs args)
	{
		string uri = args.Uri;
		if (!uri.StartsWith("https://"))
		{
			webView.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')");
			args.Cancel = true;
		}
	}

	private void ButtonGo_Click(object sender, RoutedEventArgs e)
	{
		if (webView != null && webView.CoreWebView2 != null)
		{
			webView.CoreWebView2.Navigate(addressBar.Text);
		}
	}
}
