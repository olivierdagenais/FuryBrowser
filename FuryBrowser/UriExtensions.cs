using System;

namespace FuryBrowser;

public static class UriExtensions
{
	public static Uri UpgradeToHttps(this Uri source)
	{
		var ub = new UriBuilder("https://", source.Host, -1, source.AbsolutePath);
		ub.Query = source.Query;
		ub.Fragment = source.Fragment;
		return ub.Uri;
	}
}
