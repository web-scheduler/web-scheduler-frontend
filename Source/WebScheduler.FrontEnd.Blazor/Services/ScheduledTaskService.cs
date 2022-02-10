namespace WebScheduler.FrontEnd.Blazor.Services;

using System.Net.Http.Json;
using WebScheduler.Api.ViewModels;
using WebScheduler.Abstractions.Grains.Scheduler;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

internal class ScheduledTaskService
{
    private readonly ILogger<ScheduledTaskService> logger;
    private readonly HttpClient client;

    public ScheduledTaskService(ILogger<ScheduledTaskService> logger, HttpClient client)
    {
        this.logger = logger;
        this.client = client;
    }

    public async Task<ScheduledTask> GetScheduledTaskAsync(Guid id)
    {
        try
        {
            var result = await this.client.GetFromJsonAsync<ScheduledTask>($"ScheduledTasks/{id}?api-version=1.0");
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

    public async Task<PagedCollection<ScheduledTask>?> GetPageAsync(PageOptions pageOptions)
    {
        var parameters = new Dictionary<string, string>();
        if (pageOptions.First is not null and not 0)
        {
            parameters.Add(nameof(pageOptions.First), pageOptions.First.Value.ToString());
        }

        if (pageOptions.Last is not null and not 0)
        {
            parameters.Add(nameof(pageOptions.Last), pageOptions.Last.Value.ToString());
        }
        if (!string.IsNullOrWhiteSpace(pageOptions.After))
        {
            parameters.Add(nameof(pageOptions.After), pageOptions.After);
        }
        if (!string.IsNullOrWhiteSpace(pageOptions.Before))
        {
            parameters.Add(nameof(pageOptions.Before), pageOptions.Before);
        }

        var url = QueryHelpers.AddQueryString("ScheduledTasks?api-version=1.0", parameters);

        return await this.client.GetFromJsonAsync<PagedCollection<ScheduledTask>>(url, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }

    public async Task<ScheduledTask> CreateScheduledTaskAsync(SaveScheduledTask saveScheduledTask)
    {
        try
        {
            var result = await this.client.PostAsJsonAsync($"ScheduledTasks?api-version=1.0", saveScheduledTask);
            _ = result.EnsureSuccessStatusCode();
            var scheduledTask = await result.Content.ReadFromJsonAsync<ScheduledTask>();
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

