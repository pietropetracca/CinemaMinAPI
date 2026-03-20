using _01_PrimoEsempio.DTOs.Film;
using _01_PrimoEsempio.Services.Interfaces;

public static class FilmRoutes
{
    public static void MapFilmRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/film");

        group.MapGet("/", async (string? titolo, string? genere, int? anno, int? registaId, IFilmService service) =>
        {
            var film = await service.GetAllAsync(titolo, genere, anno, registaId);
            return Results.Ok(film);
        });

        group.MapGet("/{id}", async (int id, IFilmService service) =>
        {
            var film = await service.GetByIdAsync(id);
            if (film is null)
                return Results.NotFound(new { messaggio = $"Film con id {id} non trovato" });

            return Results.Ok(film);
        });

        group.MapPost("/", async (FilmCreateDto dto, IFilmService service) =>
        {
            var film = await service.CreateAsync(dto);
            return Results.Created($"/film/{film.Id}", film);
        });

        group.MapPut("/{id}", async (int id, FilmUpdateDto dto, IFilmService service) =>
        {
            var updated = await service.UpdateAsync(id, dto);
            if (!updated)
                return Results.NotFound(new { messaggio = $"Film con id {id} non trovato" });

            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, IFilmService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            if (!deleted)
                return Results.NotFound(new { messaggio = $"Film con id {id} non trovato" });

            return Results.NoContent();
        });

        group.MapGet("/{id}/programmazione", async (int id, IFilmService service) =>
        {
            var proiezioni = await service.GetProgrammazioneAsync(id);
            return Results.Ok(proiezioni);
        });
    }
}
