using Application.Interfaces.Persistence;
using Application.Interfaces.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Resource;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Web.Controllers;

[Authorize(Roles = "Administrator")] // Domyślnie wymagaj admina, ale nadpisz dla listy/szczegółów
public class ResourceController : Controller
{
    private readonly IResourceService _resourceService;
    private readonly IResourceTypeService _resourceTypeService;
    private readonly IBookingService _bookingService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ResourceController(IResourceService resourceService, IResourceTypeService resourceTypeService, IBookingService bookingService, IWebHostEnvironment webHostEnvironment)
    {
        _resourceService = resourceService;
        _resourceTypeService = resourceTypeService;
        _bookingService = bookingService;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: /Resource
    // GET: /Resource
    public async Task<IActionResult> Index(string sortOrder, string searchString, Guid? resourceTypeId)
    {
        ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
        ViewData["CapacitySortParm"] = sortOrder == "Capacity" ? "capacity_desc" : "Capacity";
        ViewData["LocationSortParm"] = sortOrder == "Location" ? "location_desc" : "Location";
        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentType"] = resourceTypeId;

        // Populate Resource Types Dropdown
        var resourceTypes = await _resourceTypeService.GetAllResourceTypesAsync();
        ViewBag.ResourceTypes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(resourceTypes, "Id", "Name", resourceTypeId);

        IEnumerable<ResourceDto> resources;

        if (User.IsInRole("Administrator"))
        {
            resources = await _resourceService.GetAllResourcesAsync();
        }
        else
        {
            resources = await _resourceService.GetActiveResourcesAsync();
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            resources = resources.Where(s => s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                   || (s.Description != null && s.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                   || (s.Location != null && s.Location.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
        }

        if (resourceTypeId.HasValue)
        {
            resources = resources.Where(r => r.ResourceTypeId == resourceTypeId.Value);
        }

        switch (sortOrder)
        {
            case "name_desc":
                resources = resources.OrderByDescending(s => s.Name);
                break;
            case "Capacity":
                resources = resources.OrderBy(s => s.Capacity);
                break;
            case "capacity_desc":
                resources = resources.OrderByDescending(s => s.Capacity);
                break;
            case "Location":
                resources = resources.OrderBy(s => s.Location);
                break;
            case "location_desc":
                resources = resources.OrderByDescending(s => s.Location);
                break;
            default:
                resources = resources.OrderBy(s => s.Name);
                break;
        }

        return View(resources);
    }

    // GET: /Resource/Details/{id}
    // GET: /Resource/Details/{id}
    public async Task<IActionResult> Details(Guid id)
    {
        var resource = await _resourceService.GetResourceByIdAsync(id);
        if (resource == null) return NotFound();

        if (!resource.IsActive && !User.IsInRole("Administrator"))
        {
            return NotFound();
        }

        return View(resource);
    }
    
    // GET: /Resource/Create
    public async Task<IActionResult> Create()
    {
        var resourceTypes = await _resourceTypeService.GetAllResourceTypesAsync();
        ViewBag.ResourceTypes = resourceTypes;
        return View();
    }

    // POST: /Resource/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateResourceRequestDto createDto)
    {
        if (ModelState.IsValid)
        {
            if (createDto.Image != null)
            {
                createDto.ImagePath = await UploadFile(createDto.Image);
            }

            await _resourceService.CreateResourceAsync(createDto);
            return RedirectToAction(nameof(Index));
        }
        var resourceTypes = await _resourceTypeService.GetAllResourceTypesAsync();
        ViewBag.ResourceTypes = resourceTypes;
        return View(createDto);
    }

    // GET: /Resource/Edit/{id}
    public async Task<IActionResult> Edit(Guid id)
    {
        var resource = await _resourceService.GetResourceByIdAsync(id);
        if (resource == null) return NotFound();
        
        var resourceTypes = await _resourceTypeService.GetAllResourceTypesAsync();
        ViewBag.ResourceTypes = resourceTypes;

        var updateDto = new UpdateResourceRequestDto 
        { 
            Id = resource.Id,
            Name = resource.Name, 
            Description = resource.Description,
            ResourceTypeId = resource.ResourceTypeId,
            Capacity = resource.Capacity,
            IsActive = resource.IsActive,
            ImagePath = resource.ImagePath // Bind existing image path
        };
        return View(updateDto);
    }

    // POST: /Resource/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateResourceRequestDto updateDto)
    {
        if (ModelState.IsValid)
        {
            if (updateDto.Image != null)
            {
                updateDto.ImagePath = await UploadFile(updateDto.Image);
            }

            var result = await _resourceService.UpdateResourceAsync(id, updateDto);
            if (!result) return NotFound();
            return RedirectToAction(nameof(Index));
        }
        var resourceTypes = await _resourceTypeService.GetAllResourceTypesAsync();
        ViewBag.ResourceTypes = resourceTypes;
        return View(updateDto);
    }

    // GET: /Resource/Delete/{id}
    public async Task<IActionResult> Delete(Guid id)
    {
        var resource = await _resourceService.GetResourceByIdAsync(id);
        if (resource == null) return NotFound();
        return View(resource);
    }

    // POST: /Resource/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        // Soft Delete implementation allows "deleting" resources even with history
        await _resourceService.DeleteResourceAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> UploadFile(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Niedozwolone rozszerzenie pliku.");
        }

        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "resources");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = Guid.NewGuid().ToString() + extension;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return "/images/resources/" + uniqueFileName;
    }
}