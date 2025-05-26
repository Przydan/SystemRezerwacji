using SystemRezerwacji.WebApp.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SystemRezerwacji.WebApp.Services;

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