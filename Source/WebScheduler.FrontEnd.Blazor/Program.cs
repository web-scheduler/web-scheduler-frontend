
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebScheduler.FrontEnd.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddAuthorizationCore(c => { });


//builder.Services.AddOpenidConnectPkce(settings =>
//{
//    builder.Configuration.Bind("IdentitySettings", settings);

//});
builder.Services.AddOidcAuthentication(options =>
{
    //builder.Configuration.Bind("IdentitySettings", options.ProviderOptions);
    options.ProviderOptions.Authority = "https://account.nullreference.io";
    options.ProviderOptions.RedirectUri = "https://localhost:7099/authentication/login-callback";
    options.ProviderOptions.MetadataUrl= "https://account.nullreference.io/.well-known/openid-configuration";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.ResponseType = "id_token token";
    options.ProviderOptions.ClientId= "web-scheduler-api";
});

builder.Services.AddHttpClient("web-scheduler-api", client => client.BaseAddress = new Uri(new Uri(builder.Configuration.GetValue<string>("WebScheduler:BaseUri")), "/api"))
.AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
    .ConfigureHandler(
        authorizedUrls: new[] { new Uri(new Uri(builder.Configuration.GetValue<string>("WebScheduler:BaseUri")), "/api").AbsoluteUri },
        scopes: new[] { "email", "profile" }));

await builder.Build().RunAsync();
