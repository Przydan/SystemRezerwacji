namespace Shared.DTOs.Resource;

public class UpdateResourceTypeRequestDto
{
    public int Guid { get; set; }
    public string Name { get; set; } = string.Empty;
}