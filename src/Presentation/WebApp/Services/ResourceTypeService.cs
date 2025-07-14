using System.Net.Http.Json;
using Shared.DTOs.Resource;
namespace WebApp.Services;

public class ResourceTypeService : IResourceTypeService
{
    private readonly HttpClient _httpClient;

    public ResourceTypeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResourceTypeDto>?> GetResourceTypesAsync()
    {
        try
        {
            // Upewnij się, że endpoint jest poprawny i serwer go obsługuje
            // I że jest zabezpieczony (np. [Authorize]) na serwerze, aby testować token.
            return await _httpClient.GetFromJsonAsync<List<ResourceTypeDto>>("api/ResourceTypes");
        }
        catch (HttpRequestException ex)
        {
            // Możesz tu dodać logowanie lub obsługę błędów
            Console.WriteLine($"API Call Error: {ex.Message}");
            return null;
        }
    }
}