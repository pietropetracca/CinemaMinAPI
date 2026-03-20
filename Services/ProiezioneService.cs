using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.DTOs.Proiezione;
using _01_PrimoEsempio.Models;
using _01_PrimoEsempio.Services.Interfaces;

namespace _01_PrimoEsempio.Services;

public class ProiezioneService : IProiezioneService
{
    private readonly CinemaDbContext _db;

    public ProiezioneService(CinemaDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProiezioneReadDto>> GetAllAsync(int? filmId, int? salaId, DateTime? da, DateTime? a)
    {
        var query = _db.Proiezioni.Include(p => p.Film).Include(p => p.Sala).AsQueryable();

        if (filmId.HasValue)
            query = query.Where(p => p.FilmId == filmId.Value);
        if (salaId.HasValue)
            query = query.Where(p => p.SalaId == salaId.Value);
        if (da.HasValue)
            query = query.Where(p => p.DataOra >= da.Value);
        if (a.HasValue)
            query = query.Where(p => p.DataOra <= a.Value);

        var proiezioni = await query.ToListAsync();
        return proiezioni.Select(MapToReadDto).ToList();
    }

    public async Task<ProiezioneReadDto?> GetByIdAsync(int id)
    {
        var proiezione = await _db.Proiezioni
            .Include(p => p.Film)
            .Include(p => p.Sala)
            .FirstOrDefaultAsync(p => p.Id == id);
        return proiezione is null ? null : MapToReadDto(proiezione);
    }

    public async Task<ProiezioneReadDto> CreateAsync(ProiezioneCreateDto dto)
    {
        var proiezione = new Proiezione
        {
            DataOra = dto.DataOra,
            Prezzo = dto.Prezzo,
            FilmId = dto.FilmId,
            SalaId = dto.SalaId
        };
        _db.Proiezioni.Add(proiezione);
        await _db.SaveChangesAsync();

        var film = await _db.Film.FindAsync(proiezione.FilmId);
        var sala = await _db.Sale.FindAsync(proiezione.SalaId);

        return MapToReadDto(proiezione, film!, sala!);
    }

    public async Task<bool> UpdateAsync(int id, ProiezioneUpdateDto dto)
    {
        var proiezione = await _db.Proiezioni.FindAsync(id);
        if (proiezione is null) return false;

        proiezione.DataOra = dto.DataOra;
        proiezione.Prezzo = dto.Prezzo;
        proiezione.FilmId = dto.FilmId;
        proiezione.SalaId = dto.SalaId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var proiezione = await _db.Proiezioni.FindAsync(id);
        if (proiezione is null) return false;

        _db.Proiezioni.Remove(proiezione);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProiezioneReadDto>> GetSettimanaAsync()
    {
        var oggi = DateTime.Today;
        var inizioSettimana = oggi.AddDays(-(int)oggi.DayOfWeek);
        var fineSettimana = inizioSettimana.AddDays(7);

        var proiezioni = await _db.Proiezioni
            .Include(p => p.Film)
            .Include(p => p.Sala)
            .Where(p => p.DataOra >= inizioSettimana && p.DataOra < fineSettimana)
            .ToListAsync();

        return proiezioni.Select(MapToReadDto).ToList();
    }

    private static ProiezioneReadDto MapToReadDto(Proiezione p) => new()
    {
        Id = p.Id,
        DataOra = p.DataOra,
        Prezzo = p.Prezzo,
        FilmId = p.FilmId,
        FilmTitolo = p.Film?.Titolo ?? string.Empty,
        SalaId = p.SalaId,
        SalaNome = p.Sala?.Nome ?? string.Empty,
        PostiDisponibili = p.Sala?.Capienza ?? 0
    };

    private static ProiezioneReadDto MapToReadDto(Proiezione p, Film film, Sala sala) => new()
    {
        Id = p.Id,
        DataOra = p.DataOra,
        Prezzo = p.Prezzo,
        FilmId = p.FilmId,
        FilmTitolo = film.Titolo,
        SalaId = p.SalaId,
        SalaNome = sala.Nome,
        PostiDisponibili = sala.Capienza
    };
}
