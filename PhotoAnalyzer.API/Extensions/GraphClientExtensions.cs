using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace PhotoAnalyzer.API.Extensions;

public static class GraphClientExtensions
{
    public static void AddMicrosoftGraph(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(configuration.GetSection("DownstreamApi"))
            .AddInMemoryTokenCaches();
    }

    // public static void AddMicrosoftGraph(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //         .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"))
    //         .EnableTokenAcquisitionToCallDownstreamApi()
    //         .AddMicrosoftGraph(configuration.GetSection("DownstreamApi"))
    //         .AddInMemoryTokenCaches();
    //
    //     services.Configure<JwtBearerOptions>(
    //         JwtBearerDefaults.AuthenticationScheme, options =>
    //         {
    //             options.TokenValidationParameters.ValidAudience = configuration["AzureAd:ClientId"];
    //             options.TokenValidationParameters.ValidIssuer =
    //                 $"https://login.microsoftonline.com/{configuration["AzureAd:TenantId"]}/v2.0";
    //         });
    // }
}