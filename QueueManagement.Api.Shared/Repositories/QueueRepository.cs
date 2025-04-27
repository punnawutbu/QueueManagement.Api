using Dapper;
using Npgsql;
using QueueManagement.Api.Shared.Models;

namespace QueueManagement.Api.Shared.Repositories
{
    public class QueueRepository : IQueueRepository
    {
        private readonly string _connectionString;
        public QueueRepository(string connectionString)
        { _connectionString = connectionString; }
        public async Task<Queue> GetCurrentQueueAsync()
        {
            return await _GetCurrentQueueAsync();
        }

        public async Task<Queue> TakeQueueAsync()
        {
            Queue current = new Queue();
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            var qurrent = await _GetCurrentQueueAsync();
            var currentQueue = qurrent?.QueueNumber ?? "00";

            if(currentQueue == "Z9") await ClearQueueTicketAsync();

            var nextQueue = CalculateNextQueue(currentQueue);

            DateTime createdAt = await connection.QuerySingleAsync<DateTime>(
                "INSERT INTO queue_ticket (queue_number) VALUES (@QueueNumber) RETURNING created_at",
                new { QueueNumber = nextQueue },
                transaction
            );

            await transaction.CommitAsync();
            current.CreatedAt = createdAt;
            current.QueueNumber = nextQueue;

            return current;
        }

        public async Task ClearQueueAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            await connection.ExecuteAsync(
                "UPDATE queue_running SET current_queue = '00', updated_at = CURRENT_TIMESTAMP",
                transaction: transaction
            );

            await ClearQueueTicketAsync();

            await transaction.CommitAsync();
        }

        private string CalculateNextQueue(string currentQueue)
        {
            if (string.IsNullOrEmpty(currentQueue) || currentQueue.Length != 2)
                return "A0";

            if (currentQueue == "00")
                return "A0";

            char letter = currentQueue[0];
            char number = currentQueue[1];

            if (number == '9')
            {
                letter = (char)(letter + 1);
                number = '0';
            }
            else
            {
                number = (char)(number + 1);
            }

            if (letter > 'Z') letter = 'A';

            return $"{letter}{number}";
        }
        private async Task ClearQueueTicketAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            await connection.ExecuteAsync(
                "UPDATE queue_ticket SET is_active = false",
                transaction: transaction
            );

            await transaction.CommitAsync();
        }
        private async Task<Queue> _GetCurrentQueueAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var result = await connection.QueryFirstOrDefaultAsync<Queue>(
                "SELECT queue_number AS QueueNumber, created_at AS CreatedAt FROM queue_ticket WHERE is_active ORDER BY id DESC LIMIT 1"
            );

            return result;
        }
    }
}