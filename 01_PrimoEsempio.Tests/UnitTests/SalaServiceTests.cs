using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Services;
using _01_PrimoEsempio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace _01_PrimoEsempio.Tests.UnitTests;

public class SalaServiceTests
{
    private CinemaDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CinemaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new CinemaDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsList()
    {
        using var db = CreateDbContext();
        db.Sale.Add(TestDataBuilder.SalaEntity());
        await db.SaveChangesAsync();
        var service = new SalaService(db);

        var result = await service.GetAllAsync();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmptyList()
    {
        using var db = CreateDbContext();
        var service = new SalaService(db);

        var result = await service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsSala()
    {
        using var db = CreateDbContext();
        var entity = TestDataBuilder.SalaEntity();
        db.Sale.Add(entity);
        await db.SaveChangesAsync();
        var service = new SalaService(db);

        var result = await service.GetByIdAsync(entity.Id);

        result.Should().NotBeNull();
        result!.Nome.Should().Be(entity.Nome);
        result.Capienza.Should().Be(entity.Capienza);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        using var db = CreateDbContext();
        var service = new SalaService(db);

        var result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Valid_ReturnsSala()
    {
        using var db = CreateDbContext();
        var service = new SalaService(db);
        var dto = TestDataBuilder.SalaCreate();

        var result = await service.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Nome.Should().Be(dto.Nome);
        result.Capienza.Should().Be(dto.Capienza);
    }

    [Fact]
    public async Task UpdateAsync_Existing_ReturnsTrue()
    {
        using var db = CreateDbContext();
        var entity = TestDataBuilder.SalaEntity();
        db.Sale.Add(entity);
        await db.SaveChangesAsync();
        var service = new SalaService(db);
        var dto = TestDataBuilder.SalaUpdate();

        var result = await service.UpdateAsync(entity.Id, dto);

        result.Should().BeTrue();
        var updated = await db.Sale.FindAsync(entity.Id);
        updated!.Nome.Should().Be(dto.Nome);
    }

    [Fact]
    public async Task UpdateAsync_NonExisting_ReturnsFalse()
    {
        using var db = CreateDbContext();
        var service = new SalaService(db);
        var dto = TestDataBuilder.SalaUpdate();

        var result = await service.UpdateAsync(999, dto);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Existing_ReturnsTrue()
    {
        using var db = CreateDbContext();
        var entity = TestDataBuilder.SalaEntity();
        db.Sale.Add(entity);
        await db.SaveChangesAsync();
        var service = new SalaService(db);

        var result = await service.DeleteAsync(entity.Id);

        result.Should().BeTrue();
        (await db.Sale.FindAsync(entity.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ReturnsFalse()
    {
        using var db = CreateDbContext();
        var service = new SalaService(db);

        var result = await service.DeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetProgrammazioneAsync_ReturnsProiezioni()
    {
        using var db = CreateDbContext();
        var regista = TestDataBuilder.RegistaEntity();
        db.Registi.Add(regista);
        await db.SaveChangesAsync();

        var film = TestDataBuilder.FilmEntity(regista.Id);
        db.Film.Add(film);
        await db.SaveChangesAsync();

        var sala = TestDataBuilder.SalaEntity();
        db.Sale.Add(sala);
        await db.SaveChangesAsync();

        var proiezione = TestDataBuilder.ProiezioneEntity(film.Id, sala.Id);
        db.Proiezioni.Add(proiezione);
        await db.SaveChangesAsync();

        var service = new SalaService(db);

        var result = await service.GetProgrammazioneAsync(sala.Id);

        result.Should().HaveCount(1);
        result[0].SalaId.Should().Be(sala.Id);
    }
}
