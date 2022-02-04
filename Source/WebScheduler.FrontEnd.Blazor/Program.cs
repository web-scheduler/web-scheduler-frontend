using ITfoxtec.Identity.BlazorWebAssembly.OpenidConnect;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebScheduler.FrontEnd.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddAuthorizationCore(c => { });

builder.Services.AddHttpClient("frontend", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
       .AddHttpMessageHandler<AccessTokenMessageHandler>();

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("frontend"));

builder.Services.AddOpenidConnectPkce(settings =>
{
    builder.Configuration.Bind("IdentitySettings", settings);
    
});


builder.Services.AddHttpClient("web-scheduler-api", client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("WebScheduler:BaseUri")))
     .AddHttpMessageHandler(sp =>
     {
         var handler = sp.GetRequiredService<AccessTokenMessageHandler>();
         builder.Configuration.Bind("IdentitySettings", handler);
         return handler;
     });

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWebAssemblyOidcSample.API"));

builder.Services.AddOpenidConnectPkce(settings =>
{
    builder.Configuration.Bind("IdentitySettings", settings);
});

await builder.Build().RunAsync();
