using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using _01_PrimoEsempio.Data;
using _01_PrimoEsempio.DTOs.Regista;
using _01_PrimoEsempio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace _01_PrimoEsempio.Tests.IntegrationTests;

public class RegistaEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistaEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Returns200()
    {
        var response = await _client.GetAsync("/registi");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_Existing_Returns200()
    {
        var dto = TestDataBuilder.RegistaCreate();
        var createResponse = await _client.PostAsJsonAsync("/registi", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<RegistaReadDto>();

        var response = await _client.GetAsync($"/registi/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RegistaReadDto>();
        result!.Nome.Should().Be(dto.Nome);
    }

    [Fact]
    public async Task GetById_NonExisting_Returns404()
    {
        var response = await _client.GetAsync("/registi/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_Valid_Returns201()
    {
        var dto = TestDataBuilder.RegistaCreate();

        var response = await _client.PostAsJsonAsync("/registi", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<RegistaReadDto>();
        result!.Nome.Should().Be(dto.Nome);
    }

    [Fact]
    public async Task Put_Existing_Returns204()
    {
        var dto = TestDataBuilder.RegistaCreate();
        var createResponse = await _client.PostAsJsonAsync("/registi", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<RegistaReadDto>();

        var updateDto = TestDataBuilder.RegistaUpdate();
        var response = await _client.PutAsJsonAsync($"/registi/{created!.Id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Put_NonExisting_Returns404()
    {
        var dto = TestDataBuilder.RegistaUpdate();

        var response = await _client.PutAsJsonAsync("/registi/999", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Existing_Returns204()
    {
        var dto = TestDataBuilder.RegistaCreate();
        var createResponse = await _client.PostAsJsonAsync("/registi", dto);
        var created = await createResponse.Content.ReadFromJsonAsync<RegistaReadDto>();

        var response = await _client.DeleteAsync($"/registi/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonExisting_Returns404()
    {
        var response = await _client.DeleteAsync("/registi/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
