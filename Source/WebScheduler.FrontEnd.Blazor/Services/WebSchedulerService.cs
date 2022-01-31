namespace WebScheduler.FrontEnd.Blazor.Services;

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

public class WebSchedulerService : IWebSchedulerService
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;
    private readonly OidcProviderOptions  oidcProviderOptions;
    private readonly IAccessTokenProvider tokenProvider;
    private readonly Uri remoteServiceBaseUrl;

    public WebSchedulerService(HttpClient httpClient, IWebAssemblyHostEnvironment hostEnvironment, IJSRuntime jsRuntime, IOptionsSnapshot<RemoteAuthenticationOptions<OidcProviderOptions>> oidcProviderOptions, IAccessTokenProvider tokenProvider)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
        this.oidcProviderOptions = oidcProviderOptions.Value.ProviderOptions;
        this.tokenProvider = tokenProvider;
        this.remoteServiceBaseUrl = new Uri(new Uri(hostEnvironment.IsDevelopment() ? "https://localhost:5001" : hostEnvironment.BaseAddress), relativeUri: hostEnvironment.IsDevelopment() ? "" : "/api/");
    }

    public async Task<string> GetScheduledTaskAsync(string scheduledTaskId)
    {
        var uri = new Uri(this.remoteServiceBaseUrl, $"scheduledtasks/{scheduledTaskId}?api-version=1.0");
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.SetBrowserRequestMode(BrowserRequestMode.Cors);
        var tokenResult = await this.tokenProvider.RequestAccessToken().ConfigureAwait(false);

        if (tokenResult.TryGetToken(out var token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await this.GetJwtTokenAsync().ConfigureAwait(false));
            var response = await this.httpClient.SendAsync(request).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        throw new Exception("Unable to get token.");
    }
    /// <summary>
    /// Gets the raw JWT id_token.
    /// </summary>
    /// <returns>The JWT Token from the OIDC id_token</returns>
    public ValueTask<string> GetJwtTokenAsync() => this.jsRuntime.InvokeAsync<string>("getOidToken", $"oidc.user:{this.oidcProviderOptions.Authority}:{this.oidcProviderOptions.ClientId}");
}
