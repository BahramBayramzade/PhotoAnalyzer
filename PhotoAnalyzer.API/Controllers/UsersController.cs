using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using PhotoAnalyzer.API.DTOs;
using PhotoAnalyzer.API.Graph;

namespace PhotoAnalyzer.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IGraphProfileClient _graphProfileClient;

    public UsersController(IGraphProfileClient graphProfileClient)
    {
        _graphProfileClient = graphProfileClient;
    }

    [HttpGet("get-user-me")] // api/users/get-user-me
    public async Task<IActionResult> GetUserMe()
    {
        try
        {
            var user = await _graphProfileClient.GetUserAsync();

            return Ok(MapToDto(user));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            GivenName = user.GivenName,
            Surname = user.Surname,
            UserPrincipalName = user.UserPrincipalName,
            Mail = user.Mail,
            MobilePhone = user.MobilePhone,
            JobTitle = user.JobTitle,
            ProfilePictureUrl = user.Id != null
                ? $"https://graph.microsoft.com/v1.0/users/{user.Id}/photo/$value"
                : null,
        };
    }

    [HttpGet("get-user-profile-picture/{userId}")]
    public async Task<IActionResult> GetUserProfilePicture(string userId)
    {
        try
        {
          
            // Получаем изображение профиля пользователя
            var imageBytes = await _graphProfileClient.GetProfilePictureAsync(userId);
            if (imageBytes.Length == 0)
            {
                return NotFound("Profile picture not found");
            }
        
            // Возвращаем изображение как файл
            return File(imageBytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}