using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.DTOs.Film;
using _01_PrimoEsempio.DTOs.Regista;
using _01_PrimoEsempio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace _01_PrimoEsempio.Tests.IntegrationTests;

public class FilmEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FilmEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> SeedRegista()
    {
        var dto = TestDataBuilder.RegistaCreate();
        var response = await _client.PostAsJsonAsync("/registi", dto);
        var created = await response.Content.ReadFromJsonAsync<RegistaReadDto>();
        return created!.Id;
    }

    [Fact]
    public async Task GetAll_Returns200()
    {
        var response = await _client.GetAsync("/film");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_Existing_Returns200()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmCreate(registaId);
        var createResponse = await _client.PostAsJsonAsync("/film", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<FilmReadDto>();

        var response = await _client.GetAsync($"/film/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<FilmReadDto>();
        result!.Titolo.Should().Be(dto.Titolo);
    }

    [Fact]
    public async Task GetById_NonExisting_Returns404()
    {
        var response = await _client.GetAsync("/film/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Valid_Returns201()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmCreate(registaId);

        var response = await _client.PostAsJsonAsync("/film", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<FilmReadDto>();
        result!.Titolo.Should().Be(dto.Titolo);
    }

    [Fact]
    public async Task Put_Existing_Returns204()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmCreate(registaId);
        var createResponse = await _client.PostAsJsonAsync("/film", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<FilmReadDto>();

        var updateDto = TestDataBuilder.FilmUpdate(registaId);
        var response = await _client.PutAsJsonAsync($"/film/{created!.Id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Put_NonExisting_Returns404()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmUpdate(registaId);

        var response = await _client.PutAsJsonAsync("/film/999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Existing_Returns204()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmCreate(registaId);
        var createResponse = await _client.PostAsJsonAsync("/film", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<FilmReadDto>();

        var response = await _client.DeleteAsync($"/film/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonExisting_Returns404()
    {
        var response = await _client.DeleteAsync("/film/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_FilterTitolo_ReturnsFiltered()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmCreate(registaId);
        var createResponse = await _client.PostAsJsonAsync("/film", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<FilmReadDto>();

        var response = await _client.GetAsync("/film?titolo=Test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FilmReadDto>>();
        result.Should().NotBeEmpty();
        result.Should().Contain(f => f.Id == created!.Id);
    }

    [Fact]
    public async Task GetAll_FilterGenere_ReturnsFiltered()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmCreate(registaId);
        var createResponse = await _client.PostAsJsonAsync("/film", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<FilmReadDto>();

        var response = await _client.GetAsync("/film?genere=Azione");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FilmReadDto>>();
        result.Should().NotBeEmpty();
        result.Should().Contain(f => f.Id == created!.Id);
    }

    [Fact]
    public async Task GetAll_FilterAnno_ReturnsFiltered()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmCreate(registaId);
        var createResponse = await _client.PostAsJsonAsync("/film", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<FilmReadDto>();

        var response = await _client.GetAsync("/film?anno=2024");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FilmReadDto>>();
        result.Should().NotBeEmpty();
        result.Should().Contain(f => f.Id == created!.Id);
    }

    [Fact]
    public async Task GetAll_FilterRegistaId_ReturnsFiltered()
    {
        var registaId = await SeedRegista();
        var dto = TestDataBuilder.FilmCreate(registaId);
        await _client.PostAsJsonAsync("/film", dto);

        var response = await _client.GetAsync($"/film?registaId={registaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FilmReadDto>>();
        result.Should().HaveCount(1);
    }
}
