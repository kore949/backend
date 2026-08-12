namespace ProjectManagementAPI.Models
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTaskRequest
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Priority { get; set; } = "Medium";
        public int? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
    }
}