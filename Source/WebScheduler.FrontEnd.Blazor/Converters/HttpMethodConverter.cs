namespace WebScheduler.FrontEnd.Blazor.Converters;

using System.Text.Json;
using System.Text.Json.Serialization;

internal class HttpMethodConverter : JsonConverter<HttpMethod>
{
    public override HttpMethod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()?? string.Empty);

    public override void Write(Utf8JsonWriter writer, HttpMethod value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString());
}
