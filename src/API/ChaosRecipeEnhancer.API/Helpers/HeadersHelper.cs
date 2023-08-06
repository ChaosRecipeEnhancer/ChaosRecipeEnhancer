namespace ChaosRecipeEnhancer.API.Helpers;

public static class HeadersHelper
{
    public static string VersionFromUserAgent(string useragent)
    {

        var exilenceHeader = useragent.Split(' ').Where(s => s.Contains("chaosrecipeenhancer")).First();
        var version = exilenceHeader.Split('/').Last();
        return version;
    }
}