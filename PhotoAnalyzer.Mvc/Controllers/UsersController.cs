using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using PhotoAnalyzer.Mvc.Graph;
using Directory = System.IO.Directory;

namespace PhotoAnalyzer.Mvc.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IGraphProfileClient _graphProfileClient;

    public UsersController(IGraphProfileClient graphProfileClient)
    {
        _graphProfileClient = graphProfileClient;
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _graphProfileClient.GetUserAsync();
        try
        {
            var photo = await _graphProfileClient.GetProfilePictureAsync(user.Id);
            ViewBag.PhotoBase64 = Convert.ToBase64String(photo);
        }
        catch
        {
            ViewBag.PhotoBase64 = null; // Если фото нет
        }

        return View(user);
    }

    public async Task<IActionResult> Photo()
    {
        try
        {
            var user = await _graphProfileClient.GetUserAsync();
            var photo = await _graphProfileClient.GetProfilePictureAsync(user.Id);
            return File(photo, "image/jpeg");
        }
        catch
        {
            // Возвращаем стандартную иконку, если фото нет
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default-profile.png");
            return PhysicalFile(path, "image/png");
        }
    }
}