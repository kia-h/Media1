using Media1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Media1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly MediaService _mediaService;

        public FilesController(MediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var items = _mediaService.GetAllFiles()
                .Select(f => new { fileName = f.FileName, url = f.Url, size = f.Size });
            return Ok(items);
        }
    }
}
