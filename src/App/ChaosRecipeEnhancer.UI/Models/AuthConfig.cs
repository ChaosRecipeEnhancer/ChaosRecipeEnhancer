using System;

namespace ChaosRecipeEnhancer.UI.Models;

public static class AuthConfig
{
    // REF: https://www.pathofexile.com/developer/docs/authorization#clients-public
    public static readonly int DefaultTokenExpirationHours = 10;
    public static readonly Uri OAuthTokenEndpoint = new("https://chaos-recipe.com/auth/token");

    // You can only request account:* scopes - NO service:* scopes
    public static readonly string Scopes = "account:leagues account:stashes account:characters account:item_filter";

    public static string RedirectUri(string state, string codeChallenge)
        => $"https://www.pathofexile.com/oauth/authorize?client_id=chaosrecipeenhancer&response_type=code&scope={Uri.EscapeDataString(Scopes)}&state={state}&redirect_uri=https://chaos-recipe.com/auth/success&code_challenge={codeChallenge}&code_challenge_method=S256";
}
