using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Models;
using _01_PrimoEsempio.DTOs.Regista;

public static class RegistaRoutes
{
    public static void MapRegistaRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/registi");

        group.MapGet("/", async (CinemaDbContext db) =>
        {
            var registi = await db.Registi.ToListAsync();
            return Results.Ok(registi.Select(r => new RegistaReadDto
            {
                Id = r.Id,
                Nome = r.Nome,
                Cognome = r.Cognome,
                Nazionalita = r.Nazionalita
            }));
        });

        group.MapGet("/{id}", async (int id, CinemaDbContext db) =>
        {
            var regista = await db.Registi.FindAsync(id);
            if (regista is null)
                return Results.NotFound(new { messaggio = $"Regista con id {id} non trovato" });

            return Results.Ok(new RegistaReadDto
            {
                Id = regista.Id,
                Nome = regista.Nome,
                Cognome = regista.Cognome,
                Nazionalita = regista.Nazionalita
            });
        });

        group.MapPost("/", async (RegistaCreateDto dto, CinemaDbContext db) =>
        {
            var regista = new Regista
            {
                Nome = dto.Nome,
                Cognome = dto.Cognome,
                Nazionalita = dto.Nazionalita
            };
            db.Registi.Add(regista);
            await db.SaveChangesAsync();

            return Results.Created($"/registi/{regista.Id}", new RegistaReadDto
            {
                Id = regista.Id,
                Nome = regista.Nome,
                Cognome = regista.Cognome,
                Nazionalita = regista.Nazionalita
            });
        });

        group.MapPut("/{id}", async (int id, RegistaUpdateDto dto, CinemaDbContext db) =>
        {
            var regista = await db.Registi.FindAsync(id);
            if (regista is null)
                return Results.NotFound(new { messaggio = $"Regista con id {id} non trovato" });

            regista.Nome = dto.Nome;
            regista.Cognome = dto.Cognome;
            regista.Nazionalita = dto.Nazionalita;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, CinemaDbContext db) =>
        {
            var regista = await db.Registi.FindAsync(id);
            if (regista is null)
                return Results.NotFound(new { messaggio = $"Regista con id {id} non trovato" });

            db.Registi.Remove(regista);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}