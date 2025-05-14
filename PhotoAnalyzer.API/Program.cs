using Microsoft.AspNetCore.Http.Features;
using Microsoft.Identity.Web;
using PhotoAnalyzer.API.Extensions;
using PhotoAnalyzer.API.Graph;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMicrosoftGraph(builder.Configuration);

// builder.Services.AddAuthentication()
//     .AddJwtBearer(options =>
//     {
//         options.Authority = "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0";
//         options.Audience = "fb3fa063-9763-4c52-a520-92011f25aa09";
//     });
//
// builder.Services.AddAuthorizationBuilder();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IGraphFilesClient, GraphFilesClient>();
builder.Services.AddScoped<IGraphProfileClient, GraphProfileClient>();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_000_000; // 10 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();