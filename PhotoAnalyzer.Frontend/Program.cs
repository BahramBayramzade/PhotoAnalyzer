using System.IdentityModel.Tokens.Jwt;
using PhotoAnalyzer.Frontend.Authorization;
using PhotoAnalyzer.Frontend.Clients;
using PhotoAnalyzer.Frontend.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var apiUrl = builder.Configuration["ApiUrl"] ??
             throw new Exception("ApiUrl is not set");

builder.Services.AddHttpContextAccessor()
    .AddTransient<AuthorizationHandler>();

builder.Services.AddHttpClient<FilesClient>(client => client.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<AuthorizationHandler>();
builder.Services.AddHttpClient<UsersClient>(client => client.BaseAddress = new Uri(apiUrl))
    .AddHttpMessageHandler<AuthorizationHandler>();

// builder.Services
//     .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//     .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
//     {
//         options.Authority = "https://login.microsoftonline.com/d8286ac9-5463-4f4c-ade0-c3d6c1bb06c8/v2.0";
//         options.ClientId = "b695c8ec-ea62-4b97-ab4d-e17b6f78369c";
//         options.ClientSecret = "r3y8Q~~R6DMKP-JL53dy9fYkOKLlYtehGywIAbaD";
//         options.Scope.Add("api://3b5fab9d-5918-42a2-b42c-d1ae0ce2b3e4/gamestore_api.all");
//         options.ResponseType = OpenIdConnectResponseType.Code;
//         options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.SaveTokens = true;
//         options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
//     })
//     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Authority = "https://login.microsoftonline.com/consumers/v2.0";
        // options.Authority = "https://login.microsoftonline.com/d8286ac9-5463-4f4c-ade0-c3d6c1bb06c8/v2.0";
        options.ClientId = "182caf32-d700-4936-b077-4ce61bd13f4a";
        options.ClientSecret = "DTy8Q~4oFdkl6dopMZRmC-_PKrFvRvykuMfrHbl.";
        options.Scope.Add("api://fb3fa063-9763-4c52-a520-92011f25aa09/Files.Read");
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.SaveTokens = true;
        options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

// builder.Services
//     .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//     .AddOpenIdConnect(options =>
//     {
//         options.Authority = "https://login.microsoftonline.com/common/v2.0"; // Note "common"
//         options.ClientId = "182caf32-d700-4936-b077-4ce61bd13f4a";
//         options.ClientSecret = "DTy8Q~4oFdkl6dopMZRmC-_PKrFvRvykuMfrHbl.";
//         options.Scope.Add("api://fb3fa063-9763-4c52-a520-92011f25aa09/Files.Read");
//         options.Scope.Add("offline_access");
//         options.ResponseType = OpenIdConnectResponseType.Code;
//         options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.SaveTokens = true;
//         options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
//     })
//     .AddCookie();

builder.Services.AddAuthorizationBuilder();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapLoginAndLogout();

app.Run();