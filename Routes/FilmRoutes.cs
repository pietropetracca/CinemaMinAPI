using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Models;
using _01_PrimoEsempio.DTOs.Film;
using _01_PrimoEsempio.DTOs.Proiezione;

public static class FilmRoutes
{
    public static void MapFilmRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/film");

        group.MapGet("/", async (string? titolo, string? genere, int? anno, int? registaId, CinemaDbContext db) =>
        {
            var query = db.Film.Include(f => f.Regista).AsQueryable();

            if (!string.IsNullOrEmpty(titolo))
                query = query.Where(f => f.Titolo.Contains(titolo));
            if (!string.IsNullOrEmpty(genere))
                query = query.Where(f => f.Genere == genere);
            if (anno.HasValue)
                query = query.Where(f => f.Anno == anno.Value);
            if (registaId.HasValue)
                query = query.Where(f => f.RegistaId == registaId.Value);

            var film = await query.ToListAsync();
            return Results.Ok(film.Select(f => new FilmReadDto
            {
                Id = f.Id,
                Titolo = f.Titolo,
                Genere = f.Genere,
                DurataMinuti = f.DurataMinuti,
                Anno = f.Anno,
                Descrizione = f.Descrizione,
                RegistaId = f.RegistaId,
                RegistaNome = f.Regista.Nome + " " + f.Regista.Cognome
            }));
        });

        group.MapGet("/{id}", async (int id, CinemaDbContext db) =>
        {
            var film = await db.Film.Include(f => f.Regista).FirstOrDefaultAsync(f => f.Id == id);
            if (film is null)
                return Results.NotFound(new { messaggio = $"Film con id {id} non trovato" });

            return Results.Ok(new FilmReadDto
            {
                Id = film.Id,
                Titolo = film.Titolo,
                Genere = film.Genere,
                DurataMinuti = film.DurataMinuti,
                Anno = film.Anno,
                Descrizione = film.Descrizione,
                RegistaId = film.RegistaId,
                RegistaNome = film.Regista.Nome + " " + film.Regista.Cognome
            });
        });

        group.MapPost("/", async (FilmCreateDto dto, CinemaDbContext db) =>
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
            db.Film.Add(film);
            await db.SaveChangesAsync();

            var regista = await db.Registi.FindAsync(film.RegistaId);
            return Results.Created($"/film/{film.Id}", new FilmReadDto
            {
                Id = film.Id,
                Titolo = film.Titolo,
                Genere = film.Genere,
                DurataMinuti = film.DurataMinuti,
                Anno = film.Anno,
                Descrizione = film.Descrizione,
                RegistaId = film.RegistaId,
                RegistaNome = regista!.Nome + " " + regista.Cognome
            });
        });

        group.MapPut("/{id}", async (int id, FilmUpdateDto dto, CinemaDbContext db) =>
        {
            var film = await db.Film.FindAsync(id);
            if (film is null)
                return Results.NotFound(new { messaggio = $"Film con id {id} non trovato" });

            film.Titolo = dto.Titolo;
            film.Genere = dto.Genere;
            film.DurataMinuti = dto.DurataMinuti;
            film.Anno = dto.Anno;
            film.Descrizione = dto.Descrizione;
            film.RegistaId = dto.RegistaId;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, CinemaDbContext db) =>
        {
            var film = await db.Film.FindAsync(id);
            if (film is null)
                return Results.NotFound(new { messaggio = $"Film con id {id} non trovato" });

            db.Film.Remove(film);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapGet("/{id}/programmazione", async (int id, CinemaDbContext db) =>
        {
            var proiezioni = await db.Proiezioni
                .Include(p => p.Film)
                .Include(p => p.Sala)
                .Where(p => p.FilmId == id)
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