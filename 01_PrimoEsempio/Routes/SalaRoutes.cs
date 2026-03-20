using _01_PrimoEsempio.DTOs.Sala;
using _01_PrimoEsempio.Services.Interfaces;

public static class SalaRoutes
{
    public static void MapSalaRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/sale");

        group.MapGet("/", async (ISalaService service) =>
        {
            var sale = await service.GetAllAsync();
            return Results.Ok(sale);
        });

        group.MapGet("/{id}", async (int id, ISalaService service) =>
        {
            var sala = await service.GetByIdAsync(id);
            if (sala is null)
                return Results.NotFound(new { messaggio = $"Sala con id {id} non trovata" });

            return Results.Ok(sala);
        });

        group.MapPost("/", async (SalaCreateDto dto, ISalaService service) =>
        {
            var sala = await service.CreateAsync(dto);
            return Results.Created($"/sale/{sala.Id}", sala);
        });

        group.MapPut("/{id}", async (int id, SalaUpdateDto dto, ISalaService service) =>
        {
            var updated = await service.UpdateAsync(id, dto);
            if (!updated)
                return Results.NotFound(new { messaggio = $"Sala con id {id} non trovata" });

            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, ISalaService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            if (!deleted)
                return Results.NotFound(new { messaggio = $"Sala con id {id} non trovata" });

            return Results.NoContent();
        });

        group.MapGet("/{id}/programmazione", async (int id, ISalaService service) =>
        {
            var proiezioni = await service.GetProgrammazioneAsync(id);
            return Results.Ok(proiezioni);
        });
    }
}
