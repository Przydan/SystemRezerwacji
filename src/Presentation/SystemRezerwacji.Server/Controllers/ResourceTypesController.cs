// SystemRezerwacji.Server/Controllers/ResourceTypesController.cs
using MediatR; // Jeśli używasz MediatR
using Microsoft.AspNetCore.Mvc;
using SystemRezerwacji.Application.DTOs.ResourceType;
using SystemRezerwacji.Application.Features.ResourceType.Queries.GetAllResourceTypes; // Dla MediatR
// using SystemRezerwacji.Application.Services; // Jeśli używasz serwisu aplikacyjnego
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SystemRezerwacji.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourceTypesController : ControllerBase
{
    private readonly IMediator _mediator; // Dla MediatR
    // private readonly IResourceTypeService _resourceTypeService; // Dla serwisu aplikacyjnego

    public ResourceTypesController(IMediator mediator /* IResourceTypeService resourceTypeService */)
    {
        _mediator = mediator;
        // _resourceTypeService = resourceTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ResourceTypeDto>>> GetAllResourceTypes()
    {
        // Z MediatR:
        var query = new GetAllResourceTypesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);

        // Z serwisem aplikacyjnym:
        // var result = await _resourceTypeService.GetAllResourceTypesAsync();
        // return Ok(result);
    }
}