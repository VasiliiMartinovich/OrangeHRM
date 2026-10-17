using System.Diagnostics;
using OrangeHRM.Framework.Core.Api.Clients;
using OrangeHRM.Tests.API.Helpers;

namespace OrangeHRM.Tests.API;

[TestFixture]
public class AuthApiTests
{
    [Test]
    [Explicit("Interactive OAuth test")]
    public void OAuthAuthorization_ShouldReturnAuthorizationCode()
    {
        var authClient = new AuthApiClient();

        var clientId = Environment.GetEnvironmentVariable(
            "ORANGEHRM_CLIENT_ID");

        Assert.That(
            clientId,
            Is.Not.Null.And.Not.Empty,
            "Set the ORANGEHRM_CLIENT_ID environment variable first.");

        var verifier = authClient.CreateCodeVerifier();
        var challenge = authClient.CreateCodeChallenge(verifier);
        var state = authClient.CreateState();

        var authorizationUrl = authClient.BuildAuthorizationUrl(
            clientId!,
            challenge,
            state);

        using var callbackServer = new OAuthCallbackServer();

        var callbackTask = Task.Run(
            () => callbackServer.WaitForAuthorizationCode());

        Process.Start(new ProcessStartInfo
        {
            FileName = authorizationUrl,
            UseShellExecute = true
        });

        var authorizationResult = callbackTask.GetAwaiter().GetResult();

        Assert.That(
            authorizationResult.State,
            Is.EqualTo(state),
            "OAuth state must match the original value.");

        Assert.That(
            authorizationResult.Code,
            Is.Not.Null.And.Not.Empty,
            "Authorization code should be returned.");

        TestContext.WriteLine(
            "Authorization code received and state verified.");
    }
}