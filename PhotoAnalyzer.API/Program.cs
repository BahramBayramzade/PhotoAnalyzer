using Microsoft.AspNetCore.Http.Features;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using PhotoAnalyzer.API.Extensions;
using PhotoAnalyzer.API.Graph;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMicrosoftGraph(builder.Configuration);

// builder.Services.AddAuthentication()
//     .AddMicrosoftAccount(o =>
//     {
//         o.ClientId = "fb3fa063-9763-4c52-a520-92011f25aa09";
//         o.ClientSecret = "tiW8Q~TrJKAmx8XCT68vDoXYiWSulfAjv5dFWcMM";
//         // For PersonalMicrosoftAccount, it has different endpoint
//         o.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";
//         o.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
//     });

builder.Services.AddAuthorizationBuilder();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IGraphFilesClient, GraphFilesClient>();
builder.Services.AddScoped<IGraphProfileClient, GraphProfileClient>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_000_000; // 10 MB
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();