using System;
using System.Web;

namespace FuryBrowser;

public static class UriExtensions
{
	public static Uri GuessDestination(this string omnibarText)
	{
		if (omnibarText.Contains(" "))
		{
			var encodedSearchTerms = HttpUtility.UrlEncode(omnibarText);
			var ub = new UriBuilder
			{
				Scheme = "https://",
				Host = "duckduckgo.com",
				Query = $"q={encodedSearchTerms}",
			};
			return ub.Uri;
		}
		throw new NotImplementedException();
	}

	public static Uri UpgradeToHttps(this Uri source)
	{
		var ub = new UriBuilder
		{
			Scheme = "https://",
			Host = source.Host,
			Path = source.AbsolutePath,
			Query = source.Query,
			Fragment = source.Fragment,
		};
		return ub.Uri;
	}
}
