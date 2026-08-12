using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.GraphQL
{
    public class Query
    {
        // Users
        public async Task<IEnumerable<User>> GetUsers([Service] UserRepository userRepository)
        {
            return await userRepository.GetAllUsers();
        }

        public async Task<User?> GetUser(int id, [Service] UserRepository userRepository)
        {
            return await userRepository.GetUserById(id);
        }

        // Projects
        public async Task<IEnumerable<Project>> GetProjects([Service] ProjectRepository projectRepository)
        {
            return await projectRepository.GetAllProjects();
        }

        public async Task<Project?> GetProject(int id, [Service] ProjectRepository projectRepository)
        {
            return await projectRepository.GetProjectById(id);
        }

        // Tasks
        public async Task<IEnumerable<TaskModel>> GetTasks([Service] TaskRepository taskRepository, int? projectId = null)
        {
            return await taskRepository.GetAllTasks(projectId);
        }

        public async Task<TaskModel?> GetTask(int id, [Service] TaskRepository taskRepository)
        {
            return await taskRepository.GetTaskById(id);
        }
    }
}