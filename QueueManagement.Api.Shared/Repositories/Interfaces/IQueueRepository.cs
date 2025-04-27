using QueueManagement.Api.Shared.Models;

namespace QueueManagement.Api.Shared.Repositories
{
    public interface IQueueRepository
    {
        Task<Queue> GetCurrentQueueAsync();
        Task<Queue> TakeQueueAsync();
        Task ClearQueueAsync();
    }
}