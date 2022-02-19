
using System.Text.Json.Serialization;
using WebScheduler.Api.Models.ViewModels;

[JsonSerializable(typeof(HttpMethod))]
[JsonSerializable(typeof(SaveScheduledTask))]
[JsonSerializable(typeof(ScheduledTask))]
internal partial class CustomJsonSerializerContext : JsonSerializerContext
{
}
