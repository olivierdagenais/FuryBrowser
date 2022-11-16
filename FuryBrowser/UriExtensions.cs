using System;
using System.Text.RegularExpressions;
using System.Web;

namespace FuryBrowser;

public static class UriExtensions
{
	private static readonly Regex HostWithMaybePath =
		new("^(?<host>\\w+(\\.\\w+)*)(?<path>/.+)?$");

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

		var tuple = LooksLikeHostWithMaybePath(omnibarText);
		if (tuple != null)
		{
			var ub = new UriBuilder
			{
				Scheme = "https://",
				Host = tuple.Item1,
				Path = tuple.Item2,
			};
			return ub.Uri;

		}
		throw new NotImplementedException();
	}

	public static Tuple<string, string>? LooksLikeHostWithMaybePath(this string s)
	{
		var match = HostWithMaybePath.Match(s);
		if (!match.Success)
		{
			return null;
		}

		var hostValue = match.Groups["host"].Value;
		var pathValue = match.Groups["path"].Value;
		return Tuple.Create(hostValue, pathValue);
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
