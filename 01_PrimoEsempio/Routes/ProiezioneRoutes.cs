using _01_PrimoEsempio.DTOs.Proiezione;
using _01_PrimoEsempio.Services.Interfaces;

public static class ProiezioneRoutes
{
    public static void MapProiezioneRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/proiezioni");

        group.MapGet("/", async (int? filmId, int? salaId, DateTime? da, DateTime? a, IProiezioneService service) =>
        {
            var proiezioni = await service.GetAllAsync(filmId, salaId, da, a);
            return Results.Ok(proiezioni);
        });

        group.MapGet("/{id}", async (int id, IProiezioneService service) =>
        {
            var proiezione = await service.GetByIdAsync(id);
            if (proiezione is null)
                return Results.NotFound(new { messaggio = $"Proiezione con id {id} non trovata" });

            return Results.Ok(proiezione);
        });

        group.MapPost("/", async (ProiezioneCreateDto dto, IProiezioneService service) =>
        {
            var proiezione = await service.CreateAsync(dto);
            return Results.Created($"/proiezioni/{proiezione.Id}", proiezione);
        });

        group.MapPut("/{id}", async (int id, ProiezioneUpdateDto dto, IProiezioneService service) =>
        {
            var updated = await service.UpdateAsync(id, dto);
            if (!updated)
                return Results.NotFound(new { messaggio = $"Proiezione con id {id} non trovata" });

            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, IProiezioneService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            if (!deleted)
                return Results.NotFound(new { messaggio = $"Proiezione con id {id} non trovata" });

            return Results.NoContent();
        });

        group.MapGet("/settimana", async (IProiezioneService service) =>
        {
            var proiezioni = await service.GetSettimanaAsync();
            return Results.Ok(proiezioni);
        });
    }
}
