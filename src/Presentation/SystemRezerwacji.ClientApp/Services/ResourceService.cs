namespace SystemRezerwacji.ClientApp.Services
{ 
// SystemRezerwacji.ClientApp/Services/ResourceService.cs
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SystemRezerwacji.ClientApp.Models;

public class ResourceService : IResourceService
{
    private readonly HttpClient _httpClient;

    public ResourceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResourceDto>?> GetResourcesAsync()
    {
        try
        {
            // Endpoint API do pobierania zasobów, np. "/api/resources"
            // Upewnij się, że BaseAddress HttpClienta jest poprawnie skonfigurowany w Program.cs
            var response = await _httpClient.GetAsync("resources"); // Ścieżka względna do BaseAddress

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return new List<ResourceDto>(); // Zwróć pustą listę, jeśli API zwróci 204
                }
                return await response.Content.ReadFromJsonAsync<List<ResourceDto>>();
            }
            else
            {
                // Tutaj można dodać bardziej szczegółową obsługę błędów
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Błąd podczas pobierania zasobów: {response.StatusCode}, Treść: {errorContent}");
                return null; // Lub rzucić wyjątek
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wyjątek podczas pobierania zasobów: {ex.Message}");
            return null; // Lub rzucić wyjątek
        }
    }
}
}