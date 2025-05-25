using System.Net.Http.Json;
using ResourceTypeDto = SystemRezerwacji.ClientApp.Models.ResourceTypeDto;

namespace SystemRezerwacji.ClientApp.Services;

public class ResourceClientService(IHttpClientFactory clientFactory)
{
    private readonly HttpClient _http = clientFactory.CreateClient("SystemRezerwacji.ServerAPI");

    // Zmiana: Wstrzyknij IHttpClientFactory zamiast HttpClient bezpośrednio

    public Task<List<ResourceDto>?> GetResourcesAsync() =>
        _http.GetFromJsonAsync<List<ResourceDto>>("api/resources"); // Endpoint zgodny z serwerem

    public Task<List<ResourceTypeDto>?> GetResourceTypesAsync() => // Użyj DTO z warstwy Application lub stwórz dedykowane klienckie
        _http.GetFromJsonAsync<List<ResourceTypeDto>>("api/resource-types"); // Endpoint zgodny z serwerem

    // Upewnij się, że typy DTO są spójne z tym, co serwer oczekuje i zwraca
    // W `CreateResourceAsync` i `UpdateResourceAsync` serwer może oczekiwać innego DTO niż zwraca
    public async Task<bool> CreateResourceAsync(ResourceDto dto) // Rozważ CreateResourceDto
    {
        var response = await _http.PostAsJsonAsync("api/resources", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateResourceAsync(ResourceDto dto) // Rozważ UpdateResourceDto
    {
        var response = await _http.PutAsJsonAsync($"api/resources/{dto.Id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteResourceAsync(int guid) // lub Guid, jeśli ID jest Guid
    {
        var response = await _http.DeleteAsync($"api/resources/{guid}");
        return response.IsSuccessStatusCode;
    }
}