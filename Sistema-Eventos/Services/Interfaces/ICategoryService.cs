using Sistema_Eventos.DTOs;

namespace Sistema_Eventos.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto?> GetByIdAsync(Guid id);
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryResponseDto?> UpdateAsync(Guid id, CreateCategoryDto dto); // Reusamos CreateDto para update simple
        Task<bool> DeleteAsync(Guid id);
    }
}