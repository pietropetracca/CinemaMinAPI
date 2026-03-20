using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.DTOs.Sala;
using _01_PrimoEsempio.DTOs.Proiezione;
using _01_PrimoEsempio.Models;
using _01_PrimoEsempio.Services.Interfaces;

namespace _01_PrimoEsempio.Services;

public class SalaService : ISalaService
{
    private readonly CinemaDbContext _db;

    public SalaService(CinemaDbContext db)
    {
        _db = db;
    }

    public async Task<List<SalaReadDto>> GetAllAsync()
    {
        var sale = await _db.Sale.ToListAsync();
        return sale.Select(MapToReadDto).ToList();
    }

    public async Task<SalaReadDto?> GetByIdAsync(int id)
    {
        var sala = await _db.Sale.FindAsync(id);
        return sala is null ? null : MapToReadDto(sala);
    }

    public async Task<SalaReadDto> CreateAsync(SalaCreateDto dto)
    {
        var sala = new Sala
        {
            Nome = dto.Nome,
            Capienza = dto.Capienza
        };
        _db.Sale.Add(sala);
        await _db.SaveChangesAsync();
        return MapToReadDto(sala);
    }

    public async Task<bool> UpdateAsync(int id, SalaUpdateDto dto)
    {
        var sala = await _db.Sale.FindAsync(id);
        if (sala is null) return false;

        sala.Nome = dto.Nome;
        sala.Capienza = dto.Capienza;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sala = await _db.Sale.FindAsync(id);
        if (sala is null) return false;

        _db.Sale.Remove(sala);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProiezioneReadDto>> GetProgrammazioneAsync(int id)
    {
        var proiezioni = await _db.Proiezioni
            .Include(p => p.Film)
            .Include(p => p.Sala)
            .Where(p => p.SalaId == id)
            .ToListAsync();

        return proiezioni.Select(MapProiezioneToReadDto).ToList();
    }

    private static SalaReadDto MapToReadDto(Sala s) => new()
    {
        Id = s.Id,
        Nome = s.Nome,
        Capienza = s.Capienza
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
