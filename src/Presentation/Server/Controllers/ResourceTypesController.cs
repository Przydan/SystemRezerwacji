using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Persistence;
using Microsoft.AspNetCore.Authorization;
using Shared.DTOs.Resource;

namespace Server.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ResourceTypesController : Controller
    {
        private readonly IResourceTypeService _resourceTypeService;
        private readonly IResourceService _resourceService;

        public ResourceTypesController(IResourceTypeService resourceTypeService, IResourceService resourceService)
        {
            _resourceTypeService = resourceTypeService;
            _resourceService = resourceService;
        }

        // GET: /ResourceTypes
        public async Task<IActionResult> Index()
        {
            var resourceTypes = await _resourceTypeService.GetAllResourceTypesAsync();
            return View(resourceTypes);
        }

        // GET: /ResourceTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ResourceTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateResourceTypeRequestDto createDto)
        {
            if (ModelState.IsValid)
            {
                await _resourceTypeService.CreateResourceTypeAsync(createDto);
                return RedirectToAction(nameof(Index));
            }
            return View(createDto);
        }

        // GET: /ResourceTypes/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var resourceType = await _resourceTypeService.GetResourceTypeByIdAsync(id);
            if (resourceType == null) return NotFound();

            var updateDto = new UpdateResourceTypeRequestDto
            {
                Id = resourceType.Id,
                Name = resourceType.Name,
                Description = resourceType.Description,
                IconCssClass = resourceType.IconCssClass
            };
            return View(updateDto);
        }

        // POST: /ResourceTypes/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateResourceTypeRequestDto updateDto)
        {
            if (id != updateDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var success = await _resourceTypeService.UpdateResourceTypeAsync(id, updateDto);
                if (!success) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            return View(updateDto);
        }

        // GET: /ResourceTypes/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var resourceType = await _resourceTypeService.GetResourceTypeByIdAsync(id);
            if (resourceType == null) return NotFound();
            return View(resourceType);
        }

        // POST: /ResourceTypes/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // PRO TIP: Check for dependencies before deletion
            var resources = await _resourceService.GetAllResourcesAsync();
            if (resources.Any(r => r.ResourceTypeId == id))
            {
                var resourceType = await _resourceTypeService.GetResourceTypeByIdAsync(id);
                ModelState.AddModelError(string.Empty, "Nie można usunąć tego typu zasobu, ponieważ istnieją przypisane do niego zasoby. Usuń najpierw zasoby.");
                return View(resourceType);
            }

            await _resourceTypeService.DeleteResourceTypeAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
