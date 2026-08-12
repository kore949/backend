using Dapper;
using Microsoft.Data.SqlClient;

namespace ProjectManagementAPI.Data
{
    public class OtpRepository
    {
        private readonly string _connectionString;

        public OtpRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CreateOtp(string email, string otpCode, string purpose, DateTime expiresAt)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_Otp_Create",
                new { Email = email, OtpCode = otpCode, Purpose = purpose, ExpiresAt = expiresAt },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<(int OtpId, DateTime ExpiresAt, bool IsUsed)?> VerifyOtp(string email, string otpCode, string purpose)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QuerySingleOrDefaultAsync<OtpResult>(
                "usp_Otp_Verify",
                new { Email = email, OtpCode = otpCode, Purpose = purpose },
                commandType: System.Data.CommandType.StoredProcedure);

            if (result == null) return null;
            return (result.OtpId, result.ExpiresAt, result.IsUsed);
        }

        public async Task MarkOtpUsed(int otpId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_Otp_MarkUsed",
                new { OtpId = otpId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        private class OtpResult
        {
            public int OtpId { get; set; }
            public DateTime ExpiresAt { get; set; }
            public bool IsUsed { get; set; }
        }
    }
}