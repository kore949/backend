using Dapper;
using Microsoft.Data.SqlClient;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Data
{
    public class TaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateTask(CreateTaskRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QuerySingleAsync<int>(
                "usp_Task_Create",
                new
                {
                    request.ProjectId,
                    request.Title,
                    request.Description,
                    request.Priority,
                    request.AssignedTo,
                    request.DueDate
                },
                commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public async Task<TaskModel?> GetTaskById(int taskId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<TaskModel>(
                "usp_Task_GetById",
                new { TaskId = taskId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TaskModel>> GetAllTasks(int? projectId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TaskModel>(
                "usp_Task_GetAll",
                new { ProjectId = projectId },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task UpdateTask(int taskId, UpdateTaskRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_Task_Update",
                new
                {
                    TaskId = taskId,
                    request.Title,
                    request.Description,
                    request.Status,
                    request.Priority,
                    request.AssignedTo,
                    request.DueDate
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task DeleteTask(int taskId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "usp_Task_Delete",
                new { TaskId = taskId },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}