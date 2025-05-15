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
}