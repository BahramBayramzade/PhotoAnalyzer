using Microsoft.Identity.Web;

namespace PhotoAnalyzer.API.Extensions;

public static class GraphClientExtensions
{
    public static void AddMicrosoftGraphService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(configuration.GetSection("DownstreamApi"))
            .AddInMemoryTokenCaches();
    }
}