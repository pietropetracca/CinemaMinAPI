using _01_PrimoEsempio.DTOs.Film;
using _01_PrimoEsempio.DTOs.Proiezione;

namespace _01_PrimoEsempio.Services.Interfaces;

public interface IFilmService
{
    Task<List<FilmReadDto>> GetAllAsync(string? titolo, string? genere, int? anno, int? registaId);
    Task<FilmReadDto?> GetByIdAsync(int id);
    Task<FilmReadDto> CreateAsync(FilmCreateDto dto);
    Task<bool> UpdateAsync(int id, FilmUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<List<ProiezioneReadDto>> GetProgrammazioneAsync(int id);
}
