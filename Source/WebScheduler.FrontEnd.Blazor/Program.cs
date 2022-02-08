
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
        authorizedUrls: builder.Configuration.GetSection("IdentitySettings:AuthorizedUris").AsEnumerable().Select(x => x.Value).Where(j => !string.IsNullOrWhiteSpace(j)).ToArray(),
        scopes: builder.Configuration.GetSection("IdentitySettings:DefaultScopes").AsEnumerable().Select(x => x.Value).Where(j => !string.IsNullOrWhiteSpace(j)).ToArray()));

builder.Services.AddOidcAuthentication(options => builder.Configuration.Bind("IdentitySettings", options.ProviderOptions));

builder.Services.AddAuthorizationCore(c => { });

await builder.Build().RunAsync();
