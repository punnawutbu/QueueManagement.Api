using Devshift.ResponseMessage;
using QueueManagement.Api.Shared.Models;

namespace QueueManagement.Api.Shared.Facades
{
    public interface IQueueFacade
    {
        Task<ResponseMessage<Queue>> GetCurrentQueueAsync();
        Task<ResponseMessage<Queue>> GetNextQueueAsync();
        Task<ResponseMessage<string>> ResetQueueAsync();
    }
}