using Microsoft.EntityFrameworkCore;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.Services;
using _01_PrimoEsempio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace _01_PrimoEsempio.Tests.UnitTests;

public class RegistaServiceTests
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
        db.Registi.Add(TestDataBuilder.RegistaEntity());
        await db.SaveChangesAsync();
        var service = new RegistaService(db);

        var result = await service.GetAllAsync();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmptyList()
    {
        using var db = CreateDbContext();
        var service = new RegistaService(db);

        var result = await service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsRegista()
    {
        using var db = CreateDbContext();
        var entity = TestDataBuilder.RegistaEntity();
        db.Registi.Add(entity);
        await db.SaveChangesAsync();
        var service = new RegistaService(db);

        var result = await service.GetByIdAsync(entity.Id);

        result.Should().NotBeNull();
        result!.Nome.Should().Be(entity.Nome);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ReturnsNull()
    {
        using var db = CreateDbContext();
        var service = new RegistaService(db);

        var result = await service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_Valid_ReturnsRegista()
    {
        using var db = CreateDbContext();
        var service = new RegistaService(db);
        var dto = TestDataBuilder.RegistaCreate();

        var result = await service.CreateAsync(dto);

        result.Id.Should().BeGreaterThan(0);
        result.Nome.Should().Be(dto.Nome);
        result.Cognome.Should().Be(dto.Cognome);
    }

    [Fact]
    public async Task UpdateAsync_Existing_ReturnsTrue()
    {
        using var db = CreateDbContext();
        var entity = TestDataBuilder.RegistaEntity();
        db.Registi.Add(entity);
        await db.SaveChangesAsync();
        var service = new RegistaService(db);
        var dto = TestDataBuilder.RegistaUpdate();

        var result = await service.UpdateAsync(entity.Id, dto);

        result.Should().BeTrue();
        var updated = await db.Registi.FindAsync(entity.Id);
        updated!.Nome.Should().Be(dto.Nome);
    }

    [Fact]
    public async Task UpdateAsync_NonExisting_ReturnsFalse()
    {
        using var db = CreateDbContext();
        var service = new RegistaService(db);
        var dto = TestDataBuilder.RegistaUpdate();

        var result = await service.UpdateAsync(999, dto);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_Existing_ReturnsTrue()
    {
        using var db = CreateDbContext();
        var entity = TestDataBuilder.RegistaEntity();
        db.Registi.Add(entity);
        await db.SaveChangesAsync();
        var service = new RegistaService(db);

        var result = await service.DeleteAsync(entity.Id);

        result.Should().BeTrue();
        (await db.Registi.FindAsync(entity.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ReturnsFalse()
    {
        using var db = CreateDbContext();
        var service = new RegistaService(db);

        var result = await service.DeleteAsync(999);

        result.Should().BeFalse();
    }
}
