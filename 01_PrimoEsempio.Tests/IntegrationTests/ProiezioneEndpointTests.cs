using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.DTOs.Proiezione;
using _01_PrimoEsempio.DTOs.Film;
using _01_PrimoEsempio.DTOs.Regista;
using _01_PrimoEsempio.DTOs.Sala;
using _01_PrimoEsempio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace _01_PrimoEsempio.Tests.IntegrationTests;

public class ProiezioneEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProiezioneEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<(int filmId, int salaId)> SeedData()
    {
        var registaDto = TestDataBuilder.RegistaCreate();
        var registaResponse = await _client.PostAsJsonAsync("/registi", registaDto);
        var regista = await registaResponse.Content.ReadFromJsonAsync<RegistaReadDto>();

        var filmDto = TestDataBuilder.FilmCreate(regista!.Id);
        var filmResponse = await _client.PostAsJsonAsync("/film", filmDto);
        var film = await filmResponse.Content.ReadFromJsonAsync<FilmReadDto>();

        var salaDto = TestDataBuilder.SalaCreate();
        var salaResponse = await _client.PostAsJsonAsync("/sale", salaDto);
        var sala = await salaResponse.Content.ReadFromJsonAsync<SalaReadDto>();

        return (film!.Id, sala!.Id);
    }

    [Fact]
    public async Task GetAll_Returns200()
    {
        var response = await _client.GetAsync("/proiezioni");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_Existing_Returns200()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);
        var createResponse = await _client.PostAsJsonAsync("/proiezioni", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProiezioneReadDto>();

        var response = await _client.GetAsync($"/proiezioni/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ProiezioneReadDto>();
        result!.FilmId.Should().Be(filmId);
    }

    [Fact]
    public async Task GetById_NonExisting_Returns404()
    {
        var response = await _client.GetAsync("/proiezioni/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Valid_Returns201()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);

        var response = await _client.PostAsJsonAsync("/proiezioni", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ProiezioneReadDto>();
        result!.FilmId.Should().Be(filmId);
        result.SalaId.Should().Be(salaId);
    }

    [Fact]
    public async Task Put_Existing_Returns204()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);
        var createResponse = await _client.PostAsJsonAsync("/proiezioni", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProiezioneReadDto>();

        var updateDto = TestDataBuilder.ProiezioneUpdate(filmId, salaId);
        var response = await _client.PutAsJsonAsync($"/proiezioni/{created!.Id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Put_NonExisting_Returns404()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneUpdate(filmId, salaId);

        var response = await _client.PutAsJsonAsync("/proiezioni/999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Existing_Returns204()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);
        var createResponse = await _client.PostAsJsonAsync("/proiezioni", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProiezioneReadDto>();

        var response = await _client.DeleteAsync($"/proiezioni/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonExisting_Returns404()
    {
        var response = await _client.DeleteAsync("/proiezioni/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_FilterFilmId_ReturnsFiltered()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);
        await _client.PostAsJsonAsync("/proiezioni", dto);

        var response = await _client.GetAsync($"/proiezioni?filmId={filmId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ProiezioneReadDto>>();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_FilterSalaId_ReturnsFiltered()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);
        await _client.PostAsJsonAsync("/proiezioni", dto);

        var response = await _client.GetAsync($"/proiezioni?salaId={salaId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ProiezioneReadDto>>();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAll_FilterDate_ReturnsFiltered()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);
        var createResponse = await _client.PostAsJsonAsync("/proiezioni", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProiezioneReadDto>();

        var da = DateTime.Today.ToString("yyyy-MM-dd");
        var a = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/proiezioni?da={da}&a={a}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<ProiezioneReadDto>>();
        result.Should().NotBeEmpty();
        result.Should().Contain(p => p.Id == created!.Id);
    }

    [Fact]
    public async Task GetSettimana_Returns200()
    {
        var response = await _client.GetAsync("/proiezioni/settimana");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProgrammazioneFilm_Returns200()
    {
        var (filmId, salaId) = await SeedData();

        var response = await _client.GetAsync($"/film/{filmId}/programmazione");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostiDisponibili_EqualTo_Capienza()
    {
        var (filmId, salaId) = await SeedData();
        var dto = TestDataBuilder.ProiezioneCreate(filmId, salaId);
        var createResponse = await _client.PostAsJsonAsync("/proiezioni", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProiezioneReadDto>();

        created!.PostiDisponibili.Should().Be(100); // Capienza Sala Test
    }
}
