using System.Security.Cryptography;
using System.Text;

namespace OrangeHRM.Framework.Core.Api.Clients;

public class AuthApiClient
{
    private const string BaseUrl =
        "https://opensource-demo.orangehrmlive.com";

    private const string RedirectUri =
        "http://localhost:8085/callback";

    public string CreateCodeVerifier()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public string CreateCodeChallenge(string codeVerifier)
    {
        var bytes = Encoding.ASCII.GetBytes(codeVerifier);
        var hash = SHA256.HashData(bytes);

        return Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public string BuildAuthorizationUrl(
        string clientId,
        string codeChallenge,
        string state)
    {
        return $"{BaseUrl}/web/index.php/oauth2/authorize" +
               $"?response_type=code" +
               $"&client_id={Uri.EscapeDataString(clientId)}" +
               $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
               $"&code_challenge_method=S256" +
               $"&code_challenge={Uri.EscapeDataString(codeChallenge)}" +
               $"&state={Uri.EscapeDataString(state)}";
    }
    
    public string CreateState()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}