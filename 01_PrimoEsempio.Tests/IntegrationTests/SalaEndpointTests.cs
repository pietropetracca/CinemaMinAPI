using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.DTOs.Sala;
using _01_PrimoEsempio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace _01_PrimoEsempio.Tests.IntegrationTests;

public class SalaEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SalaEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Returns200()
    {
        var response = await _client.GetAsync("/sale");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_Existing_Returns200()
    {
        var dto = TestDataBuilder.SalaCreate();
        var createResponse = await _client.PostAsJsonAsync("/sale", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<SalaReadDto>();

        var response = await _client.GetAsync($"/sale/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SalaReadDto>();
        result!.Nome.Should().Be(dto.Nome);
    }

    [Fact]
    public async Task GetById_NonExisting_Returns404()
    {
        var response = await _client.GetAsync("/sale/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Valid_Returns201()
    {
        var dto = TestDataBuilder.SalaCreate();

        var response = await _client.PostAsJsonAsync("/sale", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<SalaReadDto>();
        result!.Nome.Should().Be(dto.Nome);
    }

    [Fact]
    public async Task Put_Existing_Returns204()
    {
        var dto = TestDataBuilder.SalaCreate();
        var createResponse = await _client.PostAsJsonAsync("/sale", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<SalaReadDto>();

        var updateDto = TestDataBuilder.SalaUpdate();
        var response = await _client.PutAsJsonAsync($"/sale/{created!.Id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Put_NonExisting_Returns404()
    {
        var dto = TestDataBuilder.SalaUpdate();

        var response = await _client.PutAsJsonAsync("/sale/999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Existing_Returns204()
    {
        var dto = TestDataBuilder.SalaCreate();
        var createResponse = await _client.PostAsJsonAsync("/sale", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<SalaReadDto>();

        var response = await _client.DeleteAsync($"/sale/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonExisting_Returns404()
    {
        var response = await _client.DeleteAsync("/sale/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProgrammazione_Returns200()
    {
        var dto = TestDataBuilder.SalaCreate();
        var createResponse = await _client.PostAsJsonAsync("/sale", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<SalaReadDto>();

        var response = await _client.GetAsync($"/sale/{created!.Id}/programmazione");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
