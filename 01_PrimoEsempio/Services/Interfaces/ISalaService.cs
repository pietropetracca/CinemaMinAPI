using _01_PrimoEsempio.DTOs.Sala;
using _01_PrimoEsempio.DTOs.Proiezione;

namespace _01_PrimoEsempio.Services.Interfaces;

public interface ISalaService
{
    Task<List<SalaReadDto>> GetAllAsync();
    Task<SalaReadDto?> GetByIdAsync(int id);
    Task<SalaReadDto> CreateAsync(SalaCreateDto dto);
    Task<bool> UpdateAsync(int id, SalaUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<ProiezioneReadDto>> GetProgrammazioneAsync(int id);
}
