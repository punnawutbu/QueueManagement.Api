using Devshift.ResponseMessage;
using QueueManagement.Api.Shared.Models;
using QueueManagement.Api.Shared.Repositories;

namespace QueueManagement.Api.Shared.Facades
{
    public class QueueFacade : IQueueFacade
    {
        private readonly IQueueRepository _queueRepository;

        public QueueFacade(IQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public async Task<ResponseMessage<Queue>> GetCurrentQueueAsync()
        {
            var resp = new ResponseMessage<Queue>
            {
                Result = new Queue()
            };
            var current = await _queueRepository.GetCurrentQueueAsync();

            if (current == null)
            {
                resp.Result.QueueNumber = "00";
                resp.Message = Message.Success;
                return resp;
            }

            resp.Result = current;
            resp.Message = Message.Success;
            return resp;
        }


        public async Task<ResponseMessage<Queue>> GetNextQueueAsync()
        {
            var resp = new ResponseMessage<Queue>
            {
                Result = new Queue()
            };

            resp.Result = await _queueRepository.TakeQueueAsync();
            resp.Message = Message.Success;
            return resp;
        }

        public async Task<ResponseMessage<string>> ResetQueueAsync()
        {
            var resp = new ResponseMessage<string>
            {
                Message = Message.Fail
            };

            await _queueRepository.ClearQueueAsync();
            resp.Result = "00";
            resp.Message = Message.Success;
            return resp;
        }
    }
}
