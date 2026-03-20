using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.DTOs.Regista;
using _01_PrimoEsempio.Models;
using _01_PrimoEsempio.Services.Interfaces;

namespace _01_PrimoEsempio.Services;

public class RegistaService : IRegistaService
{
    private readonly CinemaDbContext _db;

    public RegistaService(CinemaDbContext db)
    {
        _db = db;
    }

    public async Task<List<RegistaReadDto>> GetAllAsync()
    {
        var registi = await _db.Registi.ToListAsync();
        return registi.Select(MapToReadDto).ToList();
    }

    public async Task<RegistaReadDto?> GetByIdAsync(int id)
    {
        var regista = await _db.Registi.FindAsync(id);
        return regista is null ? null : MapToReadDto(regista);
    }

    public async Task<RegistaReadDto> CreateAsync(RegistaCreateDto dto)
    {
        var regista = new Regista
        {
            Nome = dto.Nome,
            Cognome = dto.Cognome,
            Nazionalita = dto.Nazionalita
        };
        _db.Registi.Add(regista);
        await _db.SaveChangesAsync();
        return MapToReadDto(regista);
    }

    public async Task<bool> UpdateAsync(int id, RegistaUpdateDto dto)
    {
        var regista = await _db.Registi.FindAsync(id);
        if (regista is null) return false;

        regista.Nome = dto.Nome;
        regista.Cognome = dto.Cognome;
        regista.Nazionalita = dto.Nazionalita;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var regista = await _db.Registi.FindAsync(id);
        if (regista is null) return false;

        _db.Registi.Remove(regista);
        await _db.SaveChangesAsync();
        return true;
    }

    private static RegistaReadDto MapToReadDto(Regista r) => new()
    {
        Id = r.Id,
        Nome = r.Nome,
        Cognome = r.Cognome,
        Nazionalita = r.Nazionalita
    };
}
