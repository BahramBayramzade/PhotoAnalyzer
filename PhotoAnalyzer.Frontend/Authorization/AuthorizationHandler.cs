using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace PhotoAnalyzer.Frontend.Authorization;

public class AuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext ??
                          throw new InvalidOperationException("No HttpContext available!");

        var accessToken = await httpContext.GetTokenAsync("access_token");

        // Логирование токена для отладки
        Console.WriteLine(string.IsNullOrEmpty(accessToken)
            ? "Access token is empty!"
            : $"Access token: {accessToken}");

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken
            );
        }

        return await base.SendAsync(request, cancellationToken);
    }
}