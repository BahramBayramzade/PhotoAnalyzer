using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Logging;
using PhotoAnalyzer.API.Extensions;
using PhotoAnalyzer.API.Graph;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMicrosoftGraphService(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IGraphFilesClient, GraphFilesClient>();
builder.Services.AddScoped<IGraphProfileClient, GraphProfileClient>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_000_000; // 10 MB
});

var app = builder.Build();

if (app.Environment.IsProduction())
{
    IdentityModelEventSource.ShowPII = true;
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();