using Microsoft.Identity.Web;

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
    //     // Get the scopes from configuration
    //     List<string> scopes = new();
    //     // scopes.Add("https://graph.microsoft.com/.default");
    //     // scopes.Add("Files.ReadWrite");
    //     // scopes.Add("Files.ReadWrite.All");
    //     // scopes.Add("Files.Read");
    //     // scopes.Add("Files.Read.All");
    //     scopes.Add("User.Read");
    //
    //     services.AddMicrosoftIdentityWebApiAuthentication(configuration)
    //         .EnableTokenAcquisitionToCallDownstreamApi(options =>
    //         {
    //             // If you need to configure any ConfidentialClientApplicationOptions, do it here
    //             // For example:
    //             options.Instance = configuration["AzureAd:Instance"];
    //             options.TenantId = configuration["AzureAd:TenantId"];
    //             options.ClientId = configuration["AzureAd:ClientId"];
    //             options.ClientSecret = configuration["AzureAd:ClientSecret"];
    //         })
    //         .AddMicrosoftGraph(configuration.GetSection("DownstreamApi"))
    //         .AddInMemoryTokenCaches();
    //
    //     // Add the scopes to the downstream API configuration
    //     services.Configure<MicrosoftGraphOptions>(options => { options.Scopes = string.Join(" ", scopes); });
    // }
}