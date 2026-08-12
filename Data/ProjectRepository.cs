using Dapper;
using Microsoft.Data.SqlClient;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Data
{
    public class ProjectRepository
    {
        private readonly string _connectionString;

        public ProjectRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateProject(CreateProjectRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QuerySingleAsync<int>(
                "usp_Project_Create",
                new
                {
                    request.Name,
                    request.Description,
                    request.StartDate,
                    request.EndDate,
                    request.CreatedBy
                },
                commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public async Task<Project?> GetProjectById(int projectId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<Project>(
                "usp_Project_GetById",
                new { ProjectId = projectId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Project>(
                "usp_Project_GetAll",
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task UpdateProject(int projectId, UpdateProjectRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_Project_Update",
                new
                {
                    ProjectId = projectId,
                    request.Name,
                    request.Description,
                    request.StartDate,
                    request.EndDate,
                    request.Status
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task DeleteProject(int projectId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_Project_Delete",
                new { ProjectId = projectId },
                commandType: System.Data.CommandType.StoredProcedure);
        }
        public async Task SetProjectManager(int projectId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_Project_SetManager",
                new { ProjectId = projectId, UserId = userId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task RemoveProjectManager(int projectId, int userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_Project_RemoveManager",
                new { ProjectId = projectId, UserId = userId },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}