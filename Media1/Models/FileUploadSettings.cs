namespace Media1.Models
{
    public class FileUploadSettings
    {
        public int MaxFileSizeMB { get; set; }

        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();

        public string[] SupportedExtensions { get; set; } = Array.Empty<string>();

        public string UploadPath { get; set; } = "wwwroot/media";
    }
}
