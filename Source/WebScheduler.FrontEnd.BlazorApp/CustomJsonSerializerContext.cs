
using System.Text.Json.Serialization;
using WebScheduler.Client.Http.Models.ViewModels;

[JsonSerializable(typeof(SaveScheduledTask))]
[JsonSerializable(typeof(SaveScheduledTask[]))]
[JsonSerializable(typeof(ScheduledTask))]
[JsonSerializable(typeof(ScheduledTask[]))]
[JsonSerializable(typeof(PageResults<ScheduledTask>))]
[JsonSerializable(typeof(PageOptions))]
internal sealed partial class CustomJsonSerializerContext : JsonSerializerContext
{
}
