namespace FuryBrowser;

/// <summary>
/// A class to test <see cref="UriExtensions"/>.
/// </summary>
[TestClass]
public class UriExtensionsTest
{
	[TestMethod]
	public void UpgradeToHttps_HttpOnly()
	{
		var input = new Uri("http://duckduckgo.com");

		var actual = UriExtensions.UpgradeToHttps(input);

		Assert.AreEqual("https://duckduckgo.com/", actual.ToString());
	}

	[TestMethod]
	public void UpgradeToHttps_HttpsAlready()
	{
		var input = new Uri("https://duckduckgo.com");

		var actual = UriExtensions.UpgradeToHttps(input);

		Assert.AreEqual("https://duckduckgo.com/", actual.ToString());
	}
}
