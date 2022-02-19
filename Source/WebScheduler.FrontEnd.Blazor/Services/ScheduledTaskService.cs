namespace WebScheduler.FrontEnd.Blazor.Services;

using System.Net.Http.Json;
using WebScheduler.Abstractions.Grains.Scheduler;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using System.Globalization;
using WebScheduler.Api.Models.ViewModels;
using System.Text.Json.Serialization;
using WebScheduler.FrontEnd.Blazor.Converters;

internal class ScheduledTaskService
{
    private readonly ILogger<ScheduledTaskService> logger;
    private readonly HttpClient client;
    private static JsonSerializerOptions jso = default!;

    public ScheduledTaskService(ILogger<ScheduledTaskService> logger, HttpClient client)
    {
        this.logger = logger;
        this.client = client;
        jso = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jso.Converters.Add(new JsonStringEnumConverter());
        jso.Converters.Add(new HttpMethodConverter());
    }

    public async Task<ScheduledTask> GetScheduledTaskAsync(Guid id)
    {
        try
        {
            var result = await this.client.GetFromJsonAsync<ScheduledTask>($"ScheduledTasks/{id}?api-version=1.0", jso);
            return result switch
            {
                null => throw new ScheduledTaskNotFoundException(id),
                _ => result
            };
        }
        catch (Exception)
        {
            throw new ScheduledTaskNotFoundException(id);
        }
    }

    public async Task<PageResults<ScheduledTask>?> GetPageAsync(PageOptions pageOptions)
    {
        pageOptions.PageSize ??= 10;
        var parameters = new Dictionary<string, string>
        {
            { nameof(pageOptions.Offset), pageOptions.Offset.ToString("D", CultureInfo.InvariantCulture) },
            { nameof(pageOptions.PageSize), pageOptions.PageSize.Value.ToString("D", CultureInfo.InvariantCulture)}
        };
        var url = QueryHelpers.AddQueryString("ScheduledTasks?api-version=1.0", parameters);

        return await this.client.GetFromJsonAsync<PageResults<ScheduledTask>>(url, jso);
    }

    public async Task<ScheduledTask> CreateScheduledTaskAsync(SaveScheduledTask saveScheduledTask)
    {
        try
        {
            var result = await this.client.PostAsJsonAsync("ScheduledTasks?api-version=1.0", saveScheduledTask, jso);
            _ = result.EnsureSuccessStatusCode();
            var scheduledTask = await result.Content.ReadFromJsonAsync<ScheduledTask>(jso);
            if (scheduledTask != null)
            {
                return scheduledTask;
            }
            throw new Exception("Unable to create scheduled task.");
        }
        catch (ScheduledTaskNotFoundException scheduledTaskNotFoundException)
        {
            this.logger.LogError(scheduledTaskNotFoundException, "Scheduled task not found");
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Scheduled task not found");
            // TODO: Handle other errors
            throw;
        }
    }
}
