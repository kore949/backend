namespace ProjectManagementAPI.Models
{
    public class ProjectMember
    {
        public int ProjectMemberId { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class AddProjectMemberRequest
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
    }
}