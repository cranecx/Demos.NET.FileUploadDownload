using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Demos.NET.FileUploadDownload.WebAPI.Controllers
{
    [ApiController]
    [Route("files")]
    public class FileController : ControllerBase
    {
        private ILogger<FileController> _logger;
        private IConfiguration _configuration;
        private Dictionary<Guid, string> FileMapping { get; set; } = [];
        public FileController(ILogger<FileController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            FileMapping.Add(Guid.Parse("ef6c248c-4b54-424c-833e-96bf50415e8f"), "Himno_a_la_Alegria.pdf");

        }

        [HttpGet("{id}")]
        public IActionResult Download(Guid id)
        {
            var fileName = FileMapping[id];
            var downloadFolder = _configuration.GetValue<string>("DownloadFolder");
            var filePath = Path.Combine(downloadFolder!, fileName);
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
                return NotFound();

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(fileInfo.Name, out string? contentType))
            {
                contentType = "application/octet-stream";
            }


            return File(fileInfo.OpenRead(), contentType);
        }

        [HttpPut]
        public IActionResult Upload([FromForm] IFormFile file)
        {
            var downloadFolder = _configuration.GetValue<string>("DownloadFolder");
            var filePath = Path.Combine(downloadFolder!, file.FileName);
            using var sourceStream = file.OpenReadStream();
            using var targetStream = System.IO.File.Create(filePath);

            sourceStream.CopyTo(targetStream);
            targetStream.Flush();

            return Ok();
        }
    }
}
