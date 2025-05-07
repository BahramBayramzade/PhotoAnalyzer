using PhotoAnalyzer.Frontend.Models;

namespace PhotoAnalyzer.Frontend.Clients;

public class UsersClient(HttpClient client)
{
    public async Task<User?> GetUserMeAsync()
    {
        var response = await client.GetAsync("api/Users/get-user-me");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<User>();
        }

        return null;
    }

    public async Task<string?> GetProfilePictureAsync(string id)
    {
        try
        {
            var response = await client.GetAsync($"api/Users/get-user-profile-picture/{id}");
            response.EnsureSuccessStatusCode();
        
            // Получаем не URL, а base64 строку изображения
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            return $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
        }
        catch
        {
            return null;
        }
    }
}