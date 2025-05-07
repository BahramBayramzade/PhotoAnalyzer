
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
        // 1. Extract access token from httpcontext
        var httpcontext = httpContextAccessor.HttpContext ??
                throw new InvalidOperationException("No HttpContext available!");

        var accessToken = await httpcontext.GetTokenAsync("access_token");

        // 2. Attach token to outgoing request
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
