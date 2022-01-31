using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebScheduler.FrontEnd.Blazor;
using WebScheduler.FrontEnd.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient<IWebSchedulerService, WebSchedulerService>()
     .AddHttpMessageHandler(services => services.GetRequiredService<AuthorizationMessageHandler>()
         .ConfigureHandler(
             authorizedUrls: new[] { builder.HostEnvironment.IsDevelopment() ? "http://localhost:5000" : builder.HostEnvironment.BaseAddress },
             scopes: new[] { "openid profile email" }
          ));

builder.Services.AddScoped(
    sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));

builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("oidc", options.ProviderOptions);
});

await builder.Build().RunAsync();
