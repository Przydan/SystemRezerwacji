using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Authorization;
using Shared.DTOs.Resource;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceTypesController : ControllerBase
    {
        private readonly IResourceTypeService _resourceTypeService;

        public ResourceTypesController(IResourceTypeService resourceTypeService)
        {
            _resourceTypeService = resourceTypeService;
        }

        // GET: api/ResourceTypes
        [HttpGet]
        public async Task<ActionResult<List<ResourceTypeDto>>> GetResourceTypes()
        {
            var resourceTypes = await _resourceTypeService.GetAllResourceTypesAsync();
            return Ok(resourceTypes);
        }

        // GET: api/ResourceTypes/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        [HttpGet("{id:guid}")] // Ograniczenie :guid
        public async Task<ActionResult<ResourceTypeDto>> GetResourceType(Guid id)
        {
            var resourceType = await _resourceTypeService.GetResourceTypeByIdAsync(id);

            if (resourceType == null)
            {
                return NotFound();
            }

            return Ok(resourceType);
        }

        // POST: api/ResourceTypes
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ResourceTypeDto>> PostResourceType(CreateResourceTypeRequestDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdResourceType = await _resourceTypeService.CreateResourceTypeAsync(createDto);
            
            // Upewnij się, że createdResourceType.Id jest Guid
            return CreatedAtAction(nameof(GetResourceType), new { id = createdResourceType.Id }, createdResourceType);
        }

        // PUT: api/ResourceTypes/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        [HttpPut("{id:guid}")] // Ograniczenie :guid
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutResourceType(Guid id, UpdateResourceTypeRequestDto updateDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _resourceTypeService.UpdateResourceTypeAsync(id, updateDto);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/ResourceTypes/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
        [HttpDelete("{id:guid}")] // Ograniczenie :guid
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteResourceType(Guid id)
        {
            var success = await _resourceTypeService.DeleteResourceTypeAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
