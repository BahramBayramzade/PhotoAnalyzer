using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace PhotoAnalyzer.API.Extensions;

public static class GraphClientExtensions
{
    public static void AddMicrosoftGraph(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(configuration,
                subscribeToJwtBearerMiddlewareDiagnosticsEvents: true)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(configuration.GetSection("DownstreamApi"))
            .AddInMemoryTokenCaches();

        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters.ValidIssuers =
            [
                "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0", // MSA issuer
                $"https://login.microsoftonline.com/{configuration["AzureAd:TenantId"]}/v2.0" // Your org issuer
            ];

            // For multi-tenant apps
            options.TokenValidationParameters.IssuerValidator = (issuer, token, parameters) =>
            {
                if (issuer.StartsWith("https://login.microsoftonline.com/"))
                    return issuer;

                throw new SecurityTokenInvalidIssuerException("Invalid issuer");
            };
        });
    }
}