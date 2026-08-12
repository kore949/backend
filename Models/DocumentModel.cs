namespace ProjectManagementAPI.Models
{
    public class DocumentModel
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSizeBytes { get; set; }
        public string StoredPath { get; set; }
        public int? ProjectId { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}