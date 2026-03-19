using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Models;
using _01_PrimoEsempio.DTOs.Proiezione;

public static class ProiezioneRoutes
{
    public static void MapProiezioneRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/proiezioni");

        group.MapGet("/", async (int? filmId, int? salaId, DateTime? da, DateTime? a, CinemaDbContext db) =>
        {
            var query = db.Proiezioni.Include(p => p.Film).Include(p => p.Sala).AsQueryable();

            if (filmId.HasValue)
                query = query.Where(p => p.FilmId == filmId.Value);
            if (salaId.HasValue)
                query = query.Where(p => p.SalaId == salaId.Value);
            if (da.HasValue)
                query = query.Where(p => p.DataOra >= da.Value);
            if (a.HasValue)
                query = query.Where(p => p.DataOra <= a.Value);

            var proiezioni = await query.ToListAsync();
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

        group.MapGet("/{id}", async (int id, CinemaDbContext db) =>
        {
            var proiezione = await db.Proiezioni.Include(p => p.Film).Include(p => p.Sala).FirstOrDefaultAsync(p => p.Id == id);
            if (proiezione is null)
                return Results.NotFound(new { messaggio = $"Proiezione con id {id} non trovata" });

            return Results.Ok(new ProiezioneReadDto
            {
                Id = proiezione.Id,
                DataOra = proiezione.DataOra,
                Prezzo = proiezione.Prezzo,
                FilmId = proiezione.FilmId,
                FilmTitolo = proiezione.Film.Titolo,
                SalaId = proiezione.SalaId,
                SalaNome = proiezione.Sala.Nome,
                PostiDisponibili = proiezione.Sala.Capienza
            });
        });

        group.MapPost("/", async (ProiezioneCreateDto dto, CinemaDbContext db) =>
        {
            var proiezione = new Proiezione
            {
                DataOra = dto.DataOra,
                Prezzo = dto.Prezzo,
                FilmId = dto.FilmId,
                SalaId = dto.SalaId
            };
            db.Proiezioni.Add(proiezione);
            await db.SaveChangesAsync();

            var film = await db.Film.FindAsync(proiezione.FilmId);
            var sala = await db.Sale.FindAsync(proiezione.SalaId);

            return Results.Created($"/proiezioni/{proiezione.Id}", new ProiezioneReadDto
            {
                Id = proiezione.Id,
                DataOra = proiezione.DataOra,
                Prezzo = proiezione.Prezzo,
                FilmId = proiezione.FilmId,
                FilmTitolo = film!.Titolo,
                SalaId = proiezione.SalaId,
                SalaNome = sala!.Nome,
                PostiDisponibili = sala.Capienza
            });
        });

        group.MapPut("/{id}", async (int id, ProiezioneUpdateDto dto, CinemaDbContext db) =>
        {
            var proiezione = await db.Proiezioni.FindAsync(id);
            if (proiezione is null)
                return Results.NotFound(new { messaggio = $"Proiezione con id {id} non trovata" });

            proiezione.DataOra = dto.DataOra;
            proiezione.Prezzo = dto.Prezzo;
            proiezione.FilmId = dto.FilmId;
            proiezione.SalaId = dto.SalaId;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, CinemaDbContext db) =>
        {
            var proiezione = await db.Proiezioni.FindAsync(id);
            if (proiezione is null)
                return Results.NotFound(new { messaggio = $"Proiezione con id {id} non trovata" });

            db.Proiezioni.Remove(proiezione);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapGet("/settimana", async (CinemaDbContext db) =>
        {
            var oggi = DateTime.Today;
            var inizioSettimana = oggi.AddDays(-(int)oggi.DayOfWeek);
            var fineSettimana = inizioSettimana.AddDays(7);

            var proiezioni = await db.Proiezioni
                .Include(p => p.Film)
                .Include(p => p.Sala)
                .Where(p => p.DataOra >= inizioSettimana && p.DataOra < fineSettimana)
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