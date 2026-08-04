using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Whisper.Application.Common.Interfaces;

namespace Whisper.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UploadsController : ControllerBase
{
    private readonly IStorageService _storageService;

    // 20 MB max
    private const long MaxFileSize = 20 * 1024 * 1024;

    public UploadsController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpPost]
    [RequestSizeLimit(100 * 1024 * 1024)] // Allow up to 100MB for video/zip at controller level, validate inside
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty.");

        // Type-based limits
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        long limit = ext switch
        {
            ".mp4" or ".mov" or ".mkv" => 100 * 1024 * 1024,
            ".mp3" or ".wav" or ".ogg" => 25 * 1024 * 1024,
            ".pdf" or ".doc" or ".docx" or ".xls" or ".xlsx" => 50 * 1024 * 1024,
            ".zip" or ".rar" or ".7z" => 100 * 1024 * 1024,
            _ => 20 * 1024 * 1024 // Images and others
        };

        if (file.Length > limit)
            return BadRequest($"File size exceeds the limit for this file type ({limit / 1024 / 1024} MB).");

        using var stream = file.OpenReadStream();
        var url = await _storageService.UploadFileAsync(file.FileName, stream, file.ContentType);

        return Ok(new { Url = url, FileName = file.FileName, FileSize = file.Length, MimeType = file.ContentType });
    }
}
