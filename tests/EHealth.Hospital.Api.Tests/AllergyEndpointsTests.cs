using System.Net;
using System.Net.Http.Json;
using EHealth.Hospital.Models;
using EHealth.Hospital.Api.Tests.Helpers;

namespace EHealth.Hospital.Api.Tests;

public class AllergyEndpointsTests : IDisposable
{
    private readonly TestFactory _factory = new();
    private readonly HttpClient _client;
    private static readonly Guid PatientId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public AllergyEndpointsTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllergies_ReturnsSeededRecord()
    {
        var response = await _client.GetAsync($"/api/allergies/patient/{PatientId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var records = await response.Content.ReadFromJsonAsync<List<AllergyRecord>>();
        Assert.NotNull(records);
        Assert.Single(records);
        Assert.Equal("Penicillin", records[0].Substance);
    }

    [Fact]
    public async Task GetAllergies_UnknownPatient_ReturnsEmptyList()
    {
        var response = await _client.GetAsync($"/api/allergies/patient/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var records = await response.Content.ReadFromJsonAsync<List<AllergyRecord>>();
        Assert.Empty(records!);
    }

    [Fact]
    public async Task PostAllergy_CreatesRecord()
    {
        var newAllergy = new AllergyRecord
        {
            PatientId = PatientId,
            Substance = "Aspirin",
            SnomedCode = "293586001",
            Source = "Test Hospital"
        };

        var response = await _client.PostAsJsonAsync("/api/allergies", newAllergy);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<AllergyRecord>();
        Assert.Equal("Aspirin", created!.Substance);
        Assert.NotEqual(Guid.Empty, created.Id);
    }

    [Fact]
    public async Task DeleteAllergy_KnownId_ReturnsNoContent()
    {
        var records = await _client.GetFromJsonAsync<List<AllergyRecord>>(
            $"/api/allergies/patient/{PatientId}");
        var id = records![0].Id;

        var response = await _client.DeleteAsync($"/api/allergies/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllergy_UnknownId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/allergies/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public void Dispose() => _factory.Dispose();
}
