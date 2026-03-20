using _01_PrimoEsempio.DTOs.Regista;

namespace _01_PrimoEsempio.Services.Interfaces;

public interface IRegistaService
{
    Task<List<RegistaReadDto>> GetAllAsync();
    Task<RegistaReadDto?> GetByIdAsync(int id);
    Task<RegistaReadDto> CreateAsync(RegistaCreateDto dto);
    Task<bool> UpdateAsync(int id, RegistaUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
