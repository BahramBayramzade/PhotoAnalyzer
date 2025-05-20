using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using PhotoAnalyzer.Mvc.Graph;

var builder = WebApplication.CreateBuilder(args);

// Измененная конфигурация аутентификации - без автоматического вызова
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration,
        subscribeToOpenIdConnectMiddlewareDiagnosticsEvents: true)
    .EnableTokenAcquisitionToCallDownstreamApi([
        "User.Read", "Files.Read", "Files.Read.All", "Files.ReadWrite", "Files.ReadWrite.All"
    ])
    .AddMicrosoftGraph(builder.Configuration.GetSection("Graph"))
    .AddInMemoryTokenCaches();

// Add services to the container.
builder.Services.AddScoped<IGraphFilesClient, GraphFilesClient>();
builder.Services.AddScoped<IGraphProfileClient, GraphProfileClient>();

builder.Services.AddControllersWithViews(_ => { }).AddMicrosoftIdentityUI();

builder.Services.Configure<MicrosoftIdentityOptions>(options =>
{
    options.Events.OnSignedOutCallbackRedirect = context =>
    {
        context.Response.Redirect("/");
        context.HandleResponse();
        return Task.CompletedTask;
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();