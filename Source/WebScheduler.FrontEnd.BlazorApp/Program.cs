using System.Diagnostics.CodeAnalysis;
using Blazor.Analytics;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebScheduler.FrontEnd.BlazorApp;
using WebScheduler.FrontEnd.BlazorApp.Services;

internal sealed class Program
{
    [RequiresUnreferencedCode("Calls Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostConfiguration.GetValue<string>(String)")]
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services
          .AddBlazorise(options => options.ChangeTextOnKeyPress = true)
          .AddBootstrap5Providers()
          .AddFontAwesomeIcons();

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddHttpClient<ScheduledTaskService>(client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("WebScheduler:BaseUri")!))
        .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: builder.Configuration.GetSection("IdentitySettings:AuthorizedUris").AsEnumerable().Select(x => x.Value).Where(j => !string.IsNullOrWhiteSpace(j)).ToArray(),
                scopes: builder.Configuration.GetSection("IdentitySettings:DefaultScopes").AsEnumerable().Select(x => x.Value).Where(j => !string.IsNullOrWhiteSpace(j)).ToArray()));

        builder.Services.AddOidcAuthentication(options => builder.Configuration.Bind("IdentitySettings", options.ProviderOptions));

        builder.Services.AddAuthorizationCore();

        builder.Services.AddGoogleAnalytics(builder.Configuration["GoogleAnalytics:UA"], builder.HostEnvironment.IsDevelopment());

        await builder.Build().RunAsync();
    }
}
