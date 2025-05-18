namespace SystemRezerwacji.ClientApp.Services
{ 
// SystemRezerwacji.ClientApp/Services/IResourceService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemRezerwacji.ClientApp.Models;

public interface IResourceService
{
    Task<List<ResourceDto>?> GetResourcesAsync();
    // W przyszłości można dodać metody do pobierania pojedynczego zasobu, filtrowania itp.
    // Task<ResourceDto?> GetResourceByIdAsync(Guid id);
}
}