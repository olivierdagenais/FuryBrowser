namespace FuryBrowser;

/// <summary>
/// A class to test <see cref="UriExtensions"/>.
/// </summary>
[TestClass]
public class UriExtensionsTest
{
	[TestMethod]
	public void GuessDestination_SpacesMeanSearch()
	{
		var input = "two words";

		var actual = UriExtensions.GuessDestination(input);

		Assert.AreEqual("https://duckduckgo.com/?q=two+words", actual.ToString());

	}

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

	[TestMethod]
	public void UpgradeToHttps_WithQueryAndFragment()
	{
		var input = new Uri("https://learn.microsoft.com/en-us/dotnet/api/system.uribuilder.-ctor?f1url=%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(System.UriBuilder.%2523ctor)%3Bk(DevLang-csharp)%26rd%3Dtrue&view=net-7.0#system-uribuilder-ctor(system-string-system-string)");

		var actual = UriExtensions.UpgradeToHttps(input);

		Assert.AreEqual(input.ToString(), actual.ToString());
	}
}
