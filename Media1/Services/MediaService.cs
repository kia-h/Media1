using Media1.Models;

namespace Media1.Services
{
    public class MediaService
    {
        private readonly IWebHostEnvironment _env;
        private readonly FileUploadSettings _settings;

        public MediaService(IWebHostEnvironment env, FileUploadSettings settings)
        {
            _env = env;
            _settings = settings;
        }

        public string GetMediaFolder()
        {
            var folder = Path.Combine(_env.WebRootPath ?? Directory.GetCurrentDirectory(), _settings.UploadPath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }

        public IEnumerable<MediaFileViewModel> GetAllFiles()
        {
            var folder = GetMediaFolder();

            // Only load .mp4 for now
            return Directory.EnumerateFiles(folder, "*.mp4", SearchOption.TopDirectoryOnly)
                .Select(fp => new MediaFileViewModel
                {
                    FileName = Path.GetFileName(fp),
                    Url = $"/{_settings.UploadPath.Replace("wwwroot/", "")}/{Path.GetFileName(fp)}",
                    Size = new FileInfo(fp).Length,
                    LastModified = File.GetLastWriteTimeUtc(fp)
                })
                .OrderByDescending(f => f.LastModified);
        }

        public bool IsAllowedExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return _settings.AllowedExtensions.Contains(ext);
        }

        public long GetMaxFileSizeBytes() => _settings.MaxFileSizeMB * 1024L * 1024L;
    }
}
