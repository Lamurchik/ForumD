using Forum.Model.Services;
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
        public IActionResult GetImage([Service] IPostsMangerService postsManger, string fileName)
        {
            var filePath = postsManger.GetFilePath(fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string? contentType))
            {
                contentType = "application/octet-stream"; // Если не удалось определить MIME
            }

            return PhysicalFile(filePath, contentType);
        }

        

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не предоставлен или пуст.");
            }

            try
            {
                var fileName = await SaveFileAsync(file);
                var url = $"{Request.Scheme}://{Request.Host}/image/{fileName}";
                return Ok(new { fileName, url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при сохранении файла: {ex.Message}");
            }
        }


        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var ext = System.IO.Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot", "Images");
            Directory.CreateDirectory(path);
            var fullPath = System.IO.Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }


    }
}
