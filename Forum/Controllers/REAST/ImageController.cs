using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Forum.Controllers.REAST
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        [HttpGet("image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = System.IO.Path.Combine("wwwroot", "Images", fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string? contentType))
            {
                contentType = "application/octet-stream"; // Если не удалось определить MIME
            }

            return PhysicalFile(filePath, contentType);
        }

    }
}
