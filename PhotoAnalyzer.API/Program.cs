using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using PhotoAnalyzer.API.Extensions;
using PhotoAnalyzer.API.Graph;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMicrosoftGraph(builder.Configuration);

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
//     .EnableTokenAcquisitionToCallDownstreamApi()
//     .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
//     .AddInMemoryTokenCaches();

builder.Services.AddLogging();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IGraphFilesClient, GraphFilesClient>();
builder.Services.AddScoped<IGraphProfileClient, GraphProfileClient>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_000_000; // 10 MB
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();