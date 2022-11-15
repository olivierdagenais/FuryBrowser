using System;

namespace FuryBrowser;

public static class UriExtensions
{
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
