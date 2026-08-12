using Dapper;
using Microsoft.Data.SqlClient;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Data
{
    public class ProjectMemberRepository
    {
        private readonly string _connectionString;

        public ProjectMemberRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task AddMember(AddProjectMemberRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_ProjectMember_Add",
                new { request.ProjectId, request.UserId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task RemoveMember(int projectId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_ProjectMember_Remove",
                new { ProjectId = projectId, UserId = userId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ProjectMember>> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ProjectMember>(
                "usp_ProjectMember_GetAll",
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ProjectMember>> GetByProject(int projectId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ProjectMember>(
                "usp_ProjectMember_GetByProject",
                new { ProjectId = projectId },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}