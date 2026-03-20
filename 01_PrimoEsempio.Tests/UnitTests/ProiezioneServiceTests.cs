using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Services;
using _01_PrimoEsempio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace _01_PrimoEsempio.Tests.UnitTests;

public class ProiezioneServiceTests
{
    private CinemaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CinemaDbContext(options);
    }

    private async Task<(int registaId, int filmId, int salaId)> SeedData(CinemaDbContext db)
    {
        var regista = TestDataBuilder.RegistaEntity();
        db.Registi.Add(regista);
        await db.SaveChangesAsync();

        var film = TestDataBuilder.FilmEntity(regista.Id);
        db.Film.Add(film);
        await db.SaveChangesAsync();

        var sala = TestDataBuilder.SalaEntity();
        db.Sale.Add(sala);
        await db.SaveChangesAsync();

        return (regista.Id, film.Id, sala.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsList()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        db.Proiezioni.Add(TestDataBuilder.ProiezioneEntity(filmId, salaId));
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var result = await service.GetAllAsync(null, null, null, null);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmptyList()
    {
        using var db = CreateDbContext();
        var service = new ProiezioneService(db);

        var result = await service.GetAllAsync(null, null, null, null);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsProiezione()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var result = await service.GetByIdAsync(entity.Id);

        result.Should().NotBeNull();
        result!.FilmId.Should().Be(filmId);
        result.SalaId.Should().Be(salaId);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        using var db = CreateDbContext();
        var service = new ProiezioneService(db);

        var result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Valid_ReturnsProiezione()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var service = new ProiezioneService(db);
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);

        var result = await service.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.FilmId.Should().Be(filmId);
        result.SalaId.Should().Be(salaId);
        result.Prezzo.Should().Be(dto.Prezzo);
    }

    [Fact]
    public async Task UpdateAsync_Existing_ReturnsTrue()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);
        var dto = TestDataBuilder.ProiezioneUpdate(filmId, salaId);

        var result = await service.UpdateAsync(entity.Id, dto);

        result.Should().BeTrue();
        var updated = await db.Proiezioni.FindAsync(entity.Id);
        updated!.Prezzo.Should().Be(dto.Prezzo);
    }

    [Fact]
    public async Task UpdateAsync_NonExisting_ReturnsFalse()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var service = new ProiezioneService(db);
        var dto = TestDataBuilder.ProiezioneUpdate(filmId, salaId);

        var result = await service.UpdateAsync(999, dto);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Existing_ReturnsTrue()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var result = await service.DeleteAsync(entity.Id);

        result.Should().BeTrue();
        (await db.Proiezioni.FindAsync(entity.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ReturnsFalse()
    {
        using var db = CreateDbContext();
        var service = new ProiezioneService(db);

        var result = await service.DeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_FilterFilmId_ReturnsFiltered()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var result = await service.GetAllAsync(filmId, null, null, null);

        result.Should().HaveCount(1);
        result[0].FilmId.Should().Be(filmId);
    }

    [Fact]
    public async Task GetAllAsync_FilterSalaId_ReturnsFiltered()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var result = await service.GetAllAsync(null, salaId, null, null);

        result.Should().HaveCount(1);
        result[0].SalaId.Should().Be(salaId);
    }

    [Fact]
    public async Task GetAllAsync_FilterDate_ReturnsFiltered()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var da = DateTime.Today;
        var a = DateTime.Today.AddDays(7);
        var result = await service.GetAllAsync(null, null, da, a);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_FilterNoMatch_ReturnsEmpty()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var result = await service.GetAllAsync(999, null, null, null);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSettimanaAsync_ReturnsWeekProiezioni()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var result = await service.GetSettimanaAsync();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task PostiDisponibili_EqualTo_Capienza()
    {
        using var db = CreateDbContext();
        var (_, filmId, salaId) = await SeedData(db);
        var entity = TestDataBuilder.ProiezioneEntity(filmId, salaId);
        db.Proiezioni.Add(entity);
        await db.SaveChangesAsync();
        var service = new ProiezioneService(db);

        var result = await service.GetByIdAsync(entity.Id);

        result.Should().NotBeNull();
        result!.PostiDisponibili.Should().Be(100); // Capienza della Sala Test
    }
}
