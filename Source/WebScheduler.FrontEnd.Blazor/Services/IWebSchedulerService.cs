namespace WebScheduler.FrontEnd.Blazor.Services;

using System.Threading.Tasks;

public interface IWebSchedulerService
{
    Task<string> GetScheduledTaskAsync(string scheduledTaskId);
}