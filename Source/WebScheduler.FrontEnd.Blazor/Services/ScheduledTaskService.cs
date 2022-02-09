namespace WebScheduler.FrontEnd.Blazor.Services;

using System.Net.Http.Json;
using WebScheduler.Api.ViewModels;
using WebScheduler.Abstractions.Grains.Scheduler;
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

    public async Task<ScheduledTask> CreateScheduledTaskAsync(Guid id, SaveScheduledTask saveScheduledTask)
    {
        try
        {
            var result = await this.client.PostAsJsonAsync($"ScheduledTasks?api-version=1.0", saveScheduledTask);
            _ = result.EnsureSuccessStatusCode();
            var scheduledTask = await result.Content.ReadFromJsonAsync<ScheduledTask>();
            return scheduledTask switch
            {
                null => throw new ScheduledTaskNotFoundException(id),
                _ => scheduledTask,
            };
        }
        catch(ScheduledTaskNotFoundException scheduledTaskNotFoundException){
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
