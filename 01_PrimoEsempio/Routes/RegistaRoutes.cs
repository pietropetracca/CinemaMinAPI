using _01_PrimoEsempio.DTOs.Regista;
using _01_PrimoEsempio.Services.Interfaces;

public static class RegistaRoutes
{
    public static void MapRegistaRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/registi");

        group.MapGet("/", async (IRegistaService service) =>
        {
            var registi = await service.GetAllAsync();
            return Results.Ok(registi);
        });

        group.MapGet("/{id}", async (int id, IRegistaService service) =>
        {
            var regista = await service.GetByIdAsync(id);
            if (regista is null)
                return Results.NotFound(new { messaggio = $"Regista con id {id} non trovato" });

            return Results.Ok(regista);
        });

        group.MapPost("/", async (RegistaCreateDto dto, IRegistaService service) =>
        {
            var regista = await service.CreateAsync(dto);
            return Results.Created($"/registi/{regista.Id}", regista);
        });

        group.MapPut("/{id}", async (int id, RegistaUpdateDto dto, IRegistaService service) =>
        {
            var updated = await service.UpdateAsync(id, dto);
            if (!updated)
                return Results.NotFound(new { messaggio = $"Regista con id {id} non trovato" });

            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, IRegistaService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            if (!deleted)
                return Results.NotFound(new { messaggio = $"Regista con id {id} non trovato" });

            return Results.NoContent();
        });
    }
}
