namespace Media1.Models
{
    public class MediaFileViewModel
    {
        public string FileName { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public long Size { get; set; }

        public DateTime LastModified { get; set; }
    }
}
