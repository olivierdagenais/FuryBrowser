using System;

namespace FuryBrowser;

public static class UriExtensions
{
	public static Uri UpgradeToHttps(this Uri source)
	{
		var ub = new UriBuilder("https://", source.Host, -1, source.PathAndQuery);
		return ub.Uri;
	}
}
