using Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Resource;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResourceController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    // GET: api/resource
    [HttpGet]
    [AllowAnonymous] // Pozostawiamy anonimowy dostęp do listy zasobów
    public async Task<IActionResult> GetResources()
    {
        var resources = await _resourceService.GetAllResourcesAsync();
        return Ok(resources);
    }

    // GET: api/resource/{id}
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetResource(Guid id)
    {
        var resource = await _resourceService.GetResourceByIdAsync(id);
        return resource != null ? Ok(resource) : NotFound();
    }
    
    // POST: api/resource
    [HttpPost]
    [Authorize(Roles = "Administrator")] // Tylko Admin może tworzyć
    public async Task<IActionResult> CreateResource([FromBody] CreateResourceRequestDto createDto)
    {
        var newResource = await _resourceService.CreateResourceAsync(createDto);
        return CreatedAtAction(nameof(GetResource), new { id = newResource.Id }, newResource);
    }

    // PUT: api/resource/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator")] // Tylko Admin może edytować
    public async Task<IActionResult> UpdateResource(Guid id, [FromBody] UpdateResourceRequestDto updateDto)
    {
        var result = await _resourceService.UpdateResourceAsync(id, updateDto);
        return result ? NoContent() : NotFound();
    }

    // DELETE: api/resource/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator")] // Tylko Admin może usuwać
    public async Task<IActionResult> DeleteResource(Guid id)
    {
        var result = await _resourceService.DeleteResourceAsync(id);
        return result ? NoContent() : NotFound();
    }
}