// using System.Net;
// using System.Text;
//
// namespace OrangeHRM.Tests.API.Helpers;
//
// public class OAuthCallbackServer : IDisposable
// {
//     private readonly HttpListener _listener = new();
//
//     public OAuthCallbackServer()
//     {
//         _listener.Prefixes.Add("http://localhost:8085/");
//     }
//
//     public (string Code, string State) WaitForAuthorizationCode()
//     {
//         _listener.Start();
//
//         var context = _listener.GetContext();
//         var request = context.Request;
//
//         string? code = request.QueryString["code"];
//         string? state = request.QueryString["state"];
//         string? error = request.QueryString["error"];
//
//         var message = !string.IsNullOrEmpty(error)
//             ? $"Authorization failed: {error}"
//             : "Authorization response received. You can close this tab.";
//
//         var responseBytes = Encoding.UTF8.GetBytes(message);
//         context.Response.ContentType = "text/plain; charset=utf-8";
//         context.Response.ContentLength64 = responseBytes.Length;
//         context.Response.OutputStream.Write(
//             responseBytes, 0, responseBytes.Length);
//         context.Response.Close();
//
//         if (!string.IsNullOrEmpty(error))
//         {
//             throw new InvalidOperationException(
//                 $"OAuth authorization failed: {error}");
//         }
//
//         if (string.IsNullOrEmpty(code))
//         {
//             throw new InvalidOperationException(
//                 "Authorization code was not returned.");
//         }
//
//         if (string.IsNullOrEmpty(state))
//         {
//             throw new InvalidOperationException(
//                 "OAuth state was not returned.");
//         }
//
//         return (code, state);
//     }
//
//     public void Dispose()
//     {
//         _listener.Close();
//     }
// }