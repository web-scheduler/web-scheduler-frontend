
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebScheduler.FrontEnd.Blazor;
using WebScheduler.FrontEnd.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");




builder.Services.AddHttpClient<ScheduledTaskService>(client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("WebScheduler:BaseUri")))
.AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(
        authorizedUrls: new[] { new Uri(builder.Configuration.GetValue<string>("WebScheduler:BaseUri")).AbsoluteUri },
        scopes: new[] { "email", "profile", "openid" }));

builder.Services.AddOidcAuthentication(options =>
{
    //builder.Configuration.Bind("IdentitySettings", options.ProviderOptions);
    options.ProviderOptions.Authority = "https://account.nullreference.io";
    options.ProviderOptions.RedirectUri = "https://scheduler.nullreference.io/authentication/login-callback";
    options.ProviderOptions.MetadataUrl = "https://account.nullreference.io/.well-known/openid-configuration";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.ResponseType = "id_token token";
    options.ProviderOptions.ClientId = "web-scheduler-api";
});
builder.Services.AddAuthorizationCore(c => { });

await builder.Build().RunAsync();
