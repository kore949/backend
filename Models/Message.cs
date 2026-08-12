namespace ProjectManagementAPI.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SendMessageRequest
    {
        public List<int> RecipientIds { get; set; }
        public string Content { get; set; }
    }
}