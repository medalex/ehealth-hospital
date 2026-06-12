using System.Net;
using System.Net.Http.Json;
using EHealth.Hospital.Models;
using EHealth.Hospital.Api.Tests.Helpers;

namespace EHealth.Hospital.Api.Tests;

public class DoctorEndpointsTests : IDisposable
{
    private readonly TestFactory _factory = new();
    private readonly HttpClient _client;

    public DoctorEndpointsTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetDoctors_ReturnsSeededDoctor()
    {
        var response = await _client.GetAsync("/api/doctors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var doctors = await response.Content.ReadFromJsonAsync<List<Doctor>>();
        Assert.NotNull(doctors);
        Assert.Single(doctors);
        Assert.Equal("Wilson", doctors[0].LastName);
    }

    [Fact]
    public async Task GetDoctor_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/doctors/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDoctor_KnownId_ReturnsDoctor()
    {
        var all = await _client.GetFromJsonAsync<List<Doctor>>("/api/doctors");
        var id = all![0].Id;

        var response = await _client.GetAsync($"/api/doctors/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var doctor = await response.Content.ReadFromJsonAsync<Doctor>();
        Assert.Equal("MED-LIC-2024-001", doctor!.LicenseNumber);
    }

    [Fact]
    public async Task DeleteDoctor_KnownId_ReturnsNoContent()
    {
        var all = await _client.GetFromJsonAsync<List<Doctor>>("/api/doctors");
        var id = all![0].Id;

        var response = await _client.DeleteAsync($"/api/doctors/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteDoctor_UnknownId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/doctors/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public void Dispose() => _factory.Dispose();
}
