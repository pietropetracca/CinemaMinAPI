using _01_PrimoEsempio.DTOs.Proiezione;

namespace _01_PrimoEsempio.Services.Interfaces;

public interface IProiezioneService
{
    Task<List<ProiezioneReadDto>> GetAllAsync(int? filmId, int? salaId, DateTime? da, DateTime? a);
    Task<ProiezioneReadDto?> GetByIdAsync(int id);
    Task<ProiezioneReadDto> CreateAsync(ProiezioneCreateDto dto);
    Task<bool> UpdateAsync(int id, ProiezioneUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<ProiezioneReadDto>> GetSettimanaAsync();
}
