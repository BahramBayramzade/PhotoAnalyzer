using System.Net.Http.Headers;
using PhotoAnalyzer.Frontend.Models;

namespace PhotoAnalyzer.Frontend.Clients;

public class FilesClient(HttpClient httpClient)
{
    public async Task<DriveItemDto[]> GetFilesAsync() // blazor app.
    {
        // var response = await httpClient.GetAsync("api/Files/get-files");
        // response.EnsureSuccessStatusCode();
        // return await response.Content.ReadFromJsonAsync<DriveItemDto[]>() ?? [];

        var response = await httpClient.GetAsync("api/Files/get-files");

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Ошибка при получении файлов: {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        return await response.Content.ReadFromJsonAsync<DriveItemDto[]>() ?? [];
    }

    public async Task UploadFileAsync(Stream fileStream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        content.Add(fileContent, "file", fileName);

        var response = await httpClient.PostAsync("api/Files/upload-file", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteFile(string id)
    {
        var response = await httpClient.DeleteAsync($"api/Files/delete-file/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<string?> GetFileThumbnail(string id)
    {
        var response = await httpClient.GetAsync($"api/Files/get-file-thumb/{id}");
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync(); // ← это URL
        return null;
    }
}