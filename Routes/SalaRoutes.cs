using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Models;
using _01_PrimoEsempio.DTOs.Sala;
using _01_PrimoEsempio.DTOs.Proiezione;
using _01_PrimoEsempio.DTOs.Proiezione;

public static class SalaRoutes
{
    public static void MapSalaRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/sale");

        group.MapGet("/", async (CinemaDbContext db) =>
        {
            var sale = await db.Sale.ToListAsync();
            return Results.Ok(sale.Select(s => new SalaReadDto
            {
                Id = s.Id,
                Nome = s.Nome,
                Capienza = s.Capienza
            }));
        });

        group.MapGet("/{id}", async (int id, CinemaDbContext db) =>
        {
            var sala = await db.Sale.FindAsync(id);
            if (sala is null)
                return Results.NotFound(new { messaggio = $"Sala con id {id} non trovata" });

            return Results.Ok(new SalaReadDto
            {
                Id = sala.Id,
                Nome = sala.Nome,
                Capienza = sala.Capienza
            });
        });

        group.MapPost("/", async (SalaCreateDto dto, CinemaDbContext db) =>
        {
            var sala = new Sala
            {
                Nome = dto.Nome,
                Capienza = dto.Capienza
            };
            db.Sale.Add(sala);
            await db.SaveChangesAsync();

            return Results.Created($"/sale/{sala.Id}", new SalaReadDto
            {
                Id = sala.Id,
                Nome = sala.Nome,
                Capienza = sala.Capienza
            });
        });

        group.MapPut("/{id}", async (int id, SalaUpdateDto dto, CinemaDbContext db) =>
        {
            var sala = await db.Sale.FindAsync(id);
            if (sala is null)
                return Results.NotFound(new { messaggio = $"Sala con id {id} non trovata" });

            sala.Nome = dto.Nome;
            sala.Capienza = dto.Capienza;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, CinemaDbContext db) =>
        {
            var sala = await db.Sale.FindAsync(id);
            if (sala is null)
                return Results.NotFound(new { messaggio = $"Sala con id {id} non trovata" });

            db.Sale.Remove(sala);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapGet("/{id}/programmazione", async (int id, CinemaDbContext db) =>
        {
            var proiezioni = await db.Proiezioni
                .Include(p => p.Film)
                .Include(p => p.Sala)
                .Where(p => p.SalaId == id)
                .ToListAsync();

            return Results.Ok(proiezioni.Select(p => new ProiezioneReadDto
            {
                Id = p.Id,
                DataOra = p.DataOra,
                Prezzo = p.Prezzo,
                FilmId = p.FilmId,
                FilmTitolo = p.Film.Titolo,
                SalaId = p.SalaId,
                SalaNome = p.Sala.Nome,
                PostiDisponibili = p.Sala.Capienza
            }));
        });
    }
}