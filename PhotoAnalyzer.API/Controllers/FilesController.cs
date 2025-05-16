using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using PhotoAnalyzer.API.DTOs;
using PhotoAnalyzer.API.Graph;

namespace PhotoAnalyzer.API.Controllers;

[Authorize]
[Route("api/[controller]")] // api/files
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IGraphFilesClient _graphFilesClient;
    private readonly ILogger<FilesController> _logger;


    public FilesController(IGraphFilesClient graphFilesClient, ILogger<FilesController> logger)
    {
        _graphFilesClient = graphFilesClient;
        _logger = logger;
    }

    [HttpGet("get-files")]
    public async Task<ActionResult<IEnumerable<DriveItem>>> GetFiles()
    {
        try
        {
            var driveItems = await _graphFilesClient.GetDriveItemsAsync();
            var items = driveItems
                .Where(item => item.File != null && item.File.MimeType.StartsWith("image/"))
                .ToList();

            return Ok(items);
        }
        catch (MicrosoftIdentityWebChallengeUserException ex)
        {
            _logger.LogError("Consent required: {ExMessage}", ex.Message);
            return StatusCode((int)HttpStatusCode.Forbidden, "Consent required. Please re-authenticate.");
        }
        catch (ServiceException ex)
        {
            _logger.LogError("Graph API error: {ExMessage}", ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving files.");
        }
    }

    [HttpPost("upload-file")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is null");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            await _graphFilesClient.UploadFileToRootAsync(file.FileName, stream);
            return Ok("File uploaded successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            _logger.LogError("Error uploading file: {ExMessage}", ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Error uploading file");
        }
    }

    [HttpDelete("delete-file/{id}")] // api/files/delete-file/{id}
    public async Task<IActionResult> DeleteFile(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("File is null");
        }

        try
        {
            await _graphFilesClient.DeleteFileAsync(id);
            return Ok("File deleted successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            _logger.LogError("Error deleting file: {ExMessage}", ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Error deleting file");
        }
    }

    [HttpGet("get-file-thumb/{id}")] // api/files/get-file-thumb/{id}
    public async Task<IActionResult> GetFileThumbnail(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("Id is null");

        try
        {
            var url = await _graphFilesClient.GetFileThumbnailUrlAsync(id); // ← строка URL
            return Ok(url); // ← верни как строку
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            _logger.LogError("Error getting thumbnail: {ExMessage}", ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, "Произошла ошибка при получении thumbnail");
        }
    }

    private static DriveItemDto MapToDto(DriveItem item)
    {
        return new DriveItemDto
        {
            Id = item.Id
                ?.Split('.')
                .FirstOrDefault(),
            Name = item.Name
                ?.Split('.')
                .FirstOrDefault(),
            Size = item.Size,
            WebUrl = item.WebUrl
        };
    }
}