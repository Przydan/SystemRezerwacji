using Application.Interfaces.Persistence;
using AutoMapper;
using Domain.Entities;
using Shared.DTOs.Resource;

namespace Application.Services
{
    public class ResourceTypeService : IResourceTypeService
    {
        private readonly IResourceTypeRepository _resourceTypeRepository;
        private readonly IMapper _mapper;

        public ResourceTypeService(IResourceTypeRepository resourceTypeRepository, IMapper mapper)
        {
            _resourceTypeRepository = resourceTypeRepository;
            _mapper = mapper;
        }

        public async Task<List<ResourceTypeDto>> GetAllResourceTypesAsync()
        {
            var resourceTypesFromDb = await _resourceTypeRepository.GetAllAsync();
            return _mapper.Map<List<ResourceTypeDto>>(resourceTypesFromDb);
        }

        public async Task<ResourceTypeDto?> GetResourceTypeByIdAsync(Guid id) // Zmieniono na Guid
        {
            var resourceType = await _resourceTypeRepository.GetByIdAsync(id); // Repozytorium musi przyjmować Guid
            if (resourceType == null)
            {
                return null;
            }
            return _mapper.Map<ResourceTypeDto>(resourceType);
        }

        public async Task<ResourceTypeDto> CreateResourceTypeAsync(CreateResourceTypeRequestDto createDto)
        {
            var resourceTypeEntity = _mapper.Map<ResourceType>(createDto);
            // ResourceType.Id zostanie prawdopodobnie wygenerowane przez bazę danych lub EF Core (np. Guid.NewGuid() jeśli nie jest auto-generowane)
            // Jeśli Id nie jest automatycznie generowane przy AddAsync, możesz je ustawić tutaj:
            // resourceTypeEntity.Id = Guid.NewGuid(); 
            var createdResourceType = await _resourceTypeRepository.AddAsync(resourceTypeEntity);
            return _mapper.Map<ResourceTypeDto>(createdResourceType);
        }

        public async Task<bool> UpdateResourceTypeAsync(Guid id, UpdateResourceTypeRequestDto updateDto) // Zmieniono na Guid
        {
            var resourceTypeToUpdate = await _resourceTypeRepository.GetByIdAsync(id); // Repozytorium musi przyjmować Guid
            if (resourceTypeToUpdate == null)
            {
                return false;
            }
            _mapper.Map(updateDto, resourceTypeToUpdate);
            await _resourceTypeRepository.UpdateAsync(resourceTypeToUpdate);
            return true;
        }

        public async Task<bool> DeleteResourceTypeAsync(Guid id) // Zmieniono na Guid
        {
            var resourceTypeToDelete = await _resourceTypeRepository.GetByIdAsync(id); // Repozytorium musi przyjmować Guid
            if (resourceTypeToDelete == null)
            {
                return false;
            }
            await _resourceTypeRepository.DeleteAsync(resourceTypeToDelete);
            return true;
        }
    }
}