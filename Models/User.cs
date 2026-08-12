namespace ProjectManagementAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public string? ProfilePhoto { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class BulkSetActiveStatusRequest
    {
        public List<int> UserIds { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateUserRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "Member";
    }

    public class UpdateUserRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}