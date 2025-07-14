using Application.Interfaces.Persistence;
using AutoMapper;
using Domain.Entities;
using Shared.DTOs.Resource;

namespace Application.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly IMapper _mapper;

        public ResourceService(IResourceRepository resourceRepository, IMapper mapper)
        {
            _resourceRepository = resourceRepository;
            _mapper = mapper;
        }

        public async Task<ResourceDto?> GetResourceByIdAsync(Guid id)
        {
            var resource = await _resourceRepository.GetByIdAsync(id);
            return resource == null ? null : _mapper.Map<ResourceDto>(resource);
        }

        public async Task<List<ResourceDto>> GetAllResourcesAsync()
        {
            var resources = await _resourceRepository.GetAllAsync();
            return _mapper.Map<List<ResourceDto>>(resources);
        }

        public async Task<ResourceDto> CreateResourceAsync(CreateResourceRequestDto createDto)
        {
            var resource = _mapper.Map<Resource>(createDto);
            await _resourceRepository.AddAsync(resource);
            // Po dodaniu, pobierz pełny obiekt, aby zwrócić DTO z nazwą typu
            var createdResource = await _resourceRepository.GetByIdAsync(resource.Id);
            return _mapper.Map<ResourceDto>(createdResource);
        }

        public async Task<bool> UpdateResourceAsync(Guid id, UpdateResourceRequestDto updateDto)
        {
            var resourceToUpdate = await _resourceRepository.GetByIdAsync(id);
            if (resourceToUpdate == null) return false;

            _mapper.Map(updateDto, resourceToUpdate);
            await _resourceRepository.UpdateAsync(resourceToUpdate);
            return true;
        }

        public async Task<bool> DeleteResourceAsync(Guid id)
        {
            var resource = await _resourceRepository.GetByIdAsync(id);
            if (resource == null) return false;

            await _resourceRepository.DeleteAsync(id);
            return true;
        }
    }
}
