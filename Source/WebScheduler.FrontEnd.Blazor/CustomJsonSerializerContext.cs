
using System.Text.Json.Serialization;
using WebScheduler.Api.Models.ViewModels;

[JsonSerializable(typeof(SaveScheduledTask))]
[JsonSerializable(typeof(SaveScheduledTask[]))]
[JsonSerializable(typeof(ScheduledTask))]
[JsonSerializable(typeof(ScheduledTask[]))]
[JsonSerializable(typeof(PageResults<ScheduledTask>))]
[JsonSerializable(typeof(PageOptions))]
internal partial class CustomJsonSerializerContext : JsonSerializerContext
{
}
