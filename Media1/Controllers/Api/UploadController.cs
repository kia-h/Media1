using Media1.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;
    private readonly FileUploadSettings _settings;
    private readonly string _uploadPath;

    public UploadController(IWebHostEnvironment env, FileUploadSettings settings, ILogger<UploadController> logger = null)
    {
        _env = env;
        _logger = logger;
        _settings = settings;

        _uploadPath = Path.Combine(_env.ContentRootPath, _settings.UploadPath ?? "wwwroot/media");
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    [HttpPost]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> Upload([FromForm] List<IFormFile> files)
    {
        _logger.LogInformation("Upload attempt with {Count} file(s)", files?.Count ?? 0);

        if (files == null || files.Count == 0)
        {
            _logger.LogWarning("No files selected");
            return BadRequest("No files selected.");
        }

        var maxSizeBytes = _settings.MaxFileSizeMB * 1024 * 1024;
        var allowedExtensions = _settings.AllowedExtensions.Select(e => e.ToLowerInvariant()).ToArray();
        var supportedExtensions = _settings.SupportedExtensions.Select(e => e.ToLowerInvariant()).ToArray();

        foreach (var file in files)
        {
            _logger.LogInformation("Processing file {FileName}, size {Size} bytes", file.FileName, file.Length);

            if (file.Length == 0)
                continue;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                return BadRequest($"File type '{ext}' is not allowed. Allowed: {string.Join(", ", allowedExtensions)}");

            if (file.Length > maxSizeBytes)
                return BadRequest($"File '{file.FileName}' exceeds the maximum allowed size of {_settings.MaxFileSizeMB} MB.");

            if (!supportedExtensions.Contains(ext))
                continue;

            var savePath = Path.Combine(_uploadPath, Path.GetFileName(file.FileName));
            using var stream = System.IO.File.Create(savePath);
            await file.CopyToAsync(stream);
        }
        _logger.LogInformation("Upload completed successfully");

        return Ok();
    }

    [HttpGet("list")]
    public IActionResult List()
    {
        var supportedExtensions = _settings.SupportedExtensions.Select(e => e.TrimStart('.')).ToArray();
        var files = Directory.EnumerateFiles(_uploadPath, "*.*")
            .Where(fp => supportedExtensions.Contains(Path.GetExtension(fp).TrimStart('.').ToLowerInvariant()))
            .Select(fp => new
            {
                FileName = Path.GetFileName(fp),
                Url = "/" + Path.Combine(_settings.UploadPath.Replace("wwwroot/", ""), Path.GetFileName(fp)).Replace("\\", "/"),
                Size = new FileInfo(fp).Length
            });

        return Ok(files);
    }
}
