using System.Net.Http.Headers;
using PhotoAnalyzer.Frontend.Models;

namespace PhotoAnalyzer.Frontend.Clients;

public class FilesClient(HttpClient httpClient)
{
    public async Task<DriveItemDto[]> GetFilesAsync(int? pageNumber = 1, int? pageSize = 10) // blazor app.
    {
        var response = await httpClient.GetAsync($"api/Files/get-files?pageNumber={pageNumber}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
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