using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoAnalyzer.Mvc.Graph;

namespace PhotoAnalyzer.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly IGraphFilesClient _graphFilesClient;

    public HomeController(IGraphFilesClient graphFilesClient)
    {
        _graphFilesClient = graphFilesClient;
    }

    public async Task<IActionResult> Index()
    {
        return await Task.FromResult(View());
    }

    [Authorize]
    public async Task<IActionResult> Photos()
    {
        var photos = await _graphFilesClient.GetDriveItemsAsync();
        return View(photos);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            await _graphFilesClient.UploadFileToRootAsync(file.FileName, stream);
            return RedirectToAction("Photos");
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var file = await _graphFilesClient.GetFileInfoAsync(id);

            await _graphFilesClient.DeleteFileAsync(id);

            TempData["DeleteMessage"] =
                $"File '{file.Name}' ({FormatFileSize(file.Size ?? 0)}) was deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting file: {ex.Message}";
        }

        return RedirectToAction("Photos");
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        while (bytes >= 1024 && order < sizes.Length - 1)
        {
            order++;
            bytes /= 1024;
        }

        return $"{bytes:0.##} {sizes[order]}";
    }

    [Authorize]
    public async Task<IActionResult> Thumbnail(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var thumbnailUrl = await _graphFilesClient.GetFileThumbnailUrlAsync(id);
        if (thumbnailUrl == null)
        {
            return NotFound();
        }

        return Redirect(thumbnailUrl);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteMultiple(string ids)
    {
        if (string.IsNullOrEmpty(ids))
        {
            TempData["ErrorMessage"] = "No files selected for deletion";
            return RedirectToAction("Photos");
        }

        var idList = ids.Split(',');
        long totalSize = 0;

        // Получаем информацию о файлах для подсчета общего размера
        foreach (var id in idList)
        {
            try
            {
                var file = await _graphFilesClient.GetFileInfoAsync(id);
                if (file != null)
                {
                    totalSize += file.Size ?? 0;
                }
            }
            catch
            {
                // ignored
            }
        }

        try
        {
            await _graphFilesClient.DeleteFilesAsync(idList);
            TempData["DeleteMessage"] =
                $"{idList.Length} files ({FormatFileSize(totalSize)}) were deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error deleting files: {ex.Message}";
        }

        return RedirectToAction("Photos");
    }
}