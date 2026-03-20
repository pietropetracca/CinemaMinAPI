using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Services;
using _01_PrimoEsempio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace _01_PrimoEsempio.Tests.UnitTests;

public class FilmServiceTests
{
    private CinemaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CinemaDbContext(options);
    }

    private async Task<int> SeedRegista(CinemaDbContext db)
    {
        var regista = TestDataBuilder.RegistaEntity();
        db.Registi.Add(regista);
        await db.SaveChangesAsync();
        return regista.Id;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsList()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        db.Film.Add(TestDataBuilder.FilmEntity(registaId));
        await db.SaveChangesAsync();
        var service = new FilmService(db);

        var result = await service.GetAllAsync(null, null, null, null);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmptyList()
    {
        using var db = CreateDbContext();
        var service = new FilmService(db);

        var result = await service.GetAllAsync(null, null, null, null);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsFilm()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var entity = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(entity);
        await db.SaveChangesAsync();
        var service = new FilmService(db);

        var result = await service.GetByIdAsync(entity.Id);

        result.Should().NotBeNull();
        result!.Titolo.Should().Be(entity.Titolo);
        result.RegistaId.Should().Be(registaId);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        using var db = CreateDbContext();
        var service = new FilmService(db);

        var result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Valid_ReturnsFilm()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var service = new FilmService(db);
        var dto = TestDataBuilder.FilmCreate(registaId);

        var result = await service.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Titolo.Should().Be(dto.Titolo);
        result.RegistaId.Should().Be(registaId);
    }

    [Fact]
    public async Task UpdateAsync_Existing_ReturnsTrue()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var entity = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(entity);
        await db.SaveChangesAsync();
        var service = new FilmService(db);
        var dto = TestDataBuilder.FilmUpdate(registaId);

        var result = await service.UpdateAsync(entity.Id, dto);

        result.Should().BeTrue();
        var updated = await db.Film.FindAsync(entity.Id);
        updated!.Titolo.Should().Be(dto.Titolo);
    }

    [Fact]
    public async Task UpdateAsync_NonExisting_ReturnsFalse()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var service = new FilmService(db);
        var dto = TestDataBuilder.FilmUpdate(registaId);

        var result = await service.UpdateAsync(999, dto);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Existing_ReturnsTrue()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var entity = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(entity);
        await db.SaveChangesAsync();
        var service = new FilmService(db);

        var result = await service.DeleteAsync(entity.Id);

        result.Should().BeTrue();
        (await db.Film.FindAsync(entity.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ReturnsFalse()
    {
        using var db = CreateDbContext();
        var service = new FilmService(db);

        var result = await service.DeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_FilterTitolo_ReturnsFiltered()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var entity = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(entity);
        await db.SaveChangesAsync();
        var service = new FilmService(db);

        var result = await service.GetAllAsync("Test", null, null, null);

        result.Should().HaveCount(1);
        result[0].Titolo.Should().Contain("Test");
    }

    [Fact]
    public async Task GetAllAsync_FilterGenere_ReturnsFiltered()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var entity = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(entity);
        await db.SaveChangesAsync();
        var service = new FilmService(db);

        var result = await service.GetAllAsync(null, "Azione", null, null);

        result.Should().HaveCount(1);
        result[0].Genere.Should().Be("Azione");
    }

    [Fact]
    public async Task GetAllAsync_FilterAnno_ReturnsFiltered()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var entity = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(entity);
        await db.SaveChangesAsync();
        var service = new FilmService(db);

        var result = await service.GetAllAsync(null, null, 2024, null);

        result.Should().HaveCount(1);
        result[0].Anno.Should().Be(2024);
    }

    [Fact]
    public async Task GetAllAsync_FilterRegistaId_ReturnsFiltered()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var entity = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(entity);
        await db.SaveChangesAsync();
        var service = new FilmService(db);

        var result = await service.GetAllAsync(null, null, null, registaId);

        result.Should().HaveCount(1);
        result[0].RegistaId.Should().Be(registaId);
    }

    [Fact]
    public async Task GetAllAsync_FilterNoMatch_ReturnsEmpty()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var entity = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(entity);
        await db.SaveChangesAsync();
        var service = new FilmService(db);

        var result = await service.GetAllAsync("NonEsiste", null, null, null);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProgrammazioneAsync_ReturnsProiezioni()
    {
        using var db = CreateDbContext();
        var registaId = await SeedRegista(db);
        var film = TestDataBuilder.FilmEntity(registaId);
        db.Film.Add(film);
        await db.SaveChangesAsync();

        var sala = TestDataBuilder.SalaEntity();
        db.Sale.Add(sala);
        await db.SaveChangesAsync();

        var proiezione = TestDataBuilder.ProiezioneEntity(film.Id, sala.Id);
        db.Proiezioni.Add(proiezione);
        await db.SaveChangesAsync();

        var service = new FilmService(db);

        var result = await service.GetProgrammazioneAsync(film.Id);

        result.Should().HaveCount(1);
        result[0].FilmId.Should().Be(film.Id);
    }
}
