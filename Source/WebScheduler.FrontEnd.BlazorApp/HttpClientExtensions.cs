namespace System.Net.Http;

using System.Text;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, IJsonPatchDocument patchDocument)
    {
        var writer = new StringWriter();
        var serializer = new JsonSerializer();
        serializer.Serialize(writer, patchDocument);
        var json = writer.ToString();

        var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");
        return await client.PatchAsync(requestUri, content);
    }

}
