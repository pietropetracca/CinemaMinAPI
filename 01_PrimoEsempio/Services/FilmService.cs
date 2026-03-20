using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.DTOs.Film;
using _01_PrimoEsempio.DTOs.Proiezione;
using _01_PrimoEsempio.Models;
using _01_PrimoEsempio.Services.Interfaces;

namespace _01_PrimoEsempio.Services;

public class FilmService : IFilmService
{
    private readonly CinemaDbContext _db;

    public FilmService(CinemaDbContext db)
    {
        _db = db;
    }

    public async Task<List<FilmReadDto>> GetAllAsync(string? titolo, string? genere, int? anno, int? registaId)
    {
        var query = _db.Film.Include(f => f.Regista).AsQueryable();

        if (!string.IsNullOrEmpty(titolo))
            query = query.Where(f => f.Titolo.Contains(titolo));
        if (!string.IsNullOrEmpty(genere))
            query = query.Where(f => f.Genere == genere);
        if (anno.HasValue)
            query = query.Where(f => f.Anno == anno.Value);
        if (registaId.HasValue)
            query = query.Where(f => f.RegistaId == registaId.Value);

        var film = await query.ToListAsync();
        return film.Select(MapToReadDto).ToList();
    }

    public async Task<FilmReadDto?> GetByIdAsync(int id)
    {
        var film = await _db.Film.Include(f => f.Regista).FirstOrDefaultAsync(f => f.Id == id);
        return film is null ? null : MapToReadDto(film);
    }

    public async Task<FilmReadDto> CreateAsync(FilmCreateDto dto)
    {
        var film = new Film
        {
            Titolo = dto.Titolo,
            Genere = dto.Genere,
            DurataMinuti = dto.DurataMinuti,
            Anno = dto.Anno,
            Descrizione = dto.Descrizione,
            RegistaId = dto.RegistaId
        };
        _db.Film.Add(film);
        await _db.SaveChangesAsync();

        var regista = await _db.Registi.FindAsync(film.RegistaId);
        return MapToReadDto(film, regista);
    }

    public async Task<bool> UpdateAsync(int id, FilmUpdateDto dto)
    {
        var film = await _db.Film.FindAsync(id);
        if (film is null) return false;

        film.Titolo = dto.Titolo;
        film.Genere = dto.Genere;
        film.DurataMinuti = dto.DurataMinuti;
        film.Anno = dto.Anno;
        film.Descrizione = dto.Descrizione;
        film.RegistaId = dto.RegistaId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var film = await _db.Film.FindAsync(id);
        if (film is null) return false;

        _db.Film.Remove(film);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProiezioneReadDto>> GetProgrammazioneAsync(int id)
    {
        var proiezioni = await _db.Proiezioni
            .Include(p => p.Film)
            .Include(p => p.Sala)
            .Where(p => p.FilmId == id)
            .ToListAsync();

        return proiezioni.Select(MapProiezioneToReadDto).ToList();
    }

    private static FilmReadDto MapToReadDto(Film f) => new()
    {
        Id = f.Id,
        Titolo = f.Titolo,
        Genere = f.Genere,
        DurataMinuti = f.DurataMinuti,
        Anno = f.Anno,
        Descrizione = f.Descrizione,
        RegistaId = f.RegistaId,
        RegistaNome = f.Regista?.Nome + " " + f.Regista?.Cognome ?? string.Empty
    };

    private static FilmReadDto MapToReadDto(Film f, Regista? r) => new()
    {
        Id = f.Id,
        Titolo = f.Titolo,
        Genere = f.Genere,
        DurataMinuti = f.DurataMinuti,
        Anno = f.Anno,
        Descrizione = f.Descrizione,
        RegistaId = f.RegistaId,
        RegistaNome = r?.Nome + " " + r?.Cognome ?? string.Empty
    };

    private static ProiezioneReadDto MapProiezioneToReadDto(Proiezione p) => new()
    {
        Id = p.Id,
        DataOra = p.DataOra,
        Prezzo = p.Prezzo,
        FilmId = p.FilmId,
        FilmTitolo = p.Film.Titolo,
        SalaId = p.SalaId,
        SalaNome = p.Sala.Nome,
        PostiDisponibili = p.Sala.Capienza
    };
}
