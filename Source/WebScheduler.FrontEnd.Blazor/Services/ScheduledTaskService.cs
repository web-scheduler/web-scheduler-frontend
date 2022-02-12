namespace WebScheduler.FrontEnd.Blazor.Services;

using System.Net.Http.Json;
using WebScheduler.Api.ViewModels;
using WebScheduler.Abstractions.Grains.Scheduler;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using System.Globalization;

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

    public async Task<PageResults<ScheduledTask>?> GetPageAsync(PageOptions pageOptions)
    {
        pageOptions.PageSize ??= 10;
        var parameters = new Dictionary<string, string>
        {
            { nameof(pageOptions.Offset), pageOptions.Offset.ToString("D", CultureInfo.InvariantCulture) },
            { nameof(pageOptions.PageSize), pageOptions.PageSize.Value.ToString("D", CultureInfo.InvariantCulture)}
        };
        var url = QueryHelpers.AddQueryString("ScheduledTasks?api-version=1.0", parameters);

        return await this.client.GetFromJsonAsync<PageResults<ScheduledTask>>(url, new JsonSerializerOptions(JsonSerializerDefaults.Web));
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

public class PageResults<T>
{
    public PageResults() => this.Items = new List<T>();
    public PageResults(List<T> items) => this.Items = items;

    /// <summary>
    /// Gets or sets the total count of items.
    /// </summary>
    /// <example>100</example>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public List<T> Items { get; set; }
}


/// <summary>
/// The options used to request a page.
/// </summary>
public class PageOptions
{
    /// <summary>
    /// Gets or sets the number of items to retrieve from the page
    /// </summary>
    /// <example>10</example>
    public int? PageSize { get; set; }

    /// <summary>
    /// Gets or sets the number of items to skip from the begining of the list.
    /// </summary>
    /// <example>10</example>
    public int Offset { get; set; }
}
