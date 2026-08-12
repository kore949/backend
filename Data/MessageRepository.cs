using Dapper;
using Microsoft.Data.SqlClient;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Data
{
    public class MessageRepository
    {
        private readonly string _connectionString;
        public MessageRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Send(int senderId, int recipientId, string content)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("usp_Message_Send",
                new { SenderId = senderId, RecipientId = recipientId, Content = content },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Message>> GetInbox(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Message>("usp_Message_GetInbox",
                new { UserId = userId }, commandType: CommandType.StoredProcedure);
        }

        public async Task MarkRead(int messageId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync("usp_Message_MarkRead",
                new { MessageId = messageId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> GetUnreadCount(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>("usp_Message_GetUnreadCount",
                new { UserId = userId }, commandType: CommandType.StoredProcedure);
        }
    }
}