using Dapper;
using Microsoft.Data.SqlClient;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateUser(CreateUserRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);
            var result = await connection.QuerySingleAsync<int>(
                "usp_User_Create",
                new
                {
                    request.FullName,
                    request.Email,
                    PasswordHash = hashedPassword,
                    request.Role
                },
                commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public async Task<User?> GetUserById(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(
                "usp_User_GetById",
                new { UserId = userId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<User>(
                "usp_User_GetAll",
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task UpdateUser(int userId, UpdateUserRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_User_Update",
                new
                {
                    UserId = userId,
                    request.FullName,
                    request.Email,
                    request.Role
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task DeleteUser(int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_User_Delete",
                new { UserId = userId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<int> BulkSetActiveStatus(List<int> userIds, bool isActive)
        {
            using var connection = new SqlConnection(_connectionString);
            var userIdsString = string.Join(",", userIds);
            var result = await connection.QuerySingleAsync<int>(
                "usp_User_BulkSetActiveStatus",
                new
                {
                    UserIds = userIdsString,
                    IsActive = isActive
                },
                commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(
                "usp_User_GetByEmail",
                new { Email = email },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task SetUserVerified(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_User_SetVerified",
                new { Email = email },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task UpdateUserPassword(string email, string newPasswordHash)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_User_UpdatePassword",
                new { Email = email, NewPasswordHash = newPasswordHash },
                commandType: System.Data.CommandType.StoredProcedure);
        }
        public async Task UpdateOwnProfile(int userId, string fullName, string? profilePhoto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_User_UpdateOwnProfile",
                new { UserId = userId, FullName = fullName, ProfilePhoto = profilePhoto },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}