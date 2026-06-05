using System.Net;
using System.Net.Http.Json;
using EHealth.Hospital.Models;
using EHealth.Hospital.Api.Tests.Helpers;

namespace EHealth.Hospital.Api.Tests;

public class PrescriptionEndpointsTests : IDisposable
{
    private readonly TestFactory _factory = new();
    private readonly HttpClient _client;

    public PrescriptionEndpointsTests()
    {
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPrescriptions_InitiallyEmpty()
    {
        var response = await _client.GetAsync("/api/prescriptions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<Prescription>>();
        Assert.Empty(list!);
    }

    [Fact]
    public async Task GetPrescription_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/prescriptions/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostPrescription_DoctorNotFound_ReturnsBadRequest()
    {
        var request = new
        {
            DoctorId = Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            DrugId = 1,
            Dosage = "500mg",
            PatientAge = 30,
            WorkflowId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/prescriptions", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostPrescription_DoctorWithoutCredential_ReturnsBadRequest()
    {
        var doctors = await _client.GetFromJsonAsync<List<Doctor>>("/api/doctors");
        var doctorId = doctors![0].Id;

        var request = new
        {
            DoctorId = doctorId,
            PatientId = Guid.NewGuid(),
            DrugId = 1,
            Dosage = "500mg",
            PatientAge = 30,
            WorkflowId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/prescriptions", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeletePrescription_UnknownId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/prescriptions/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    public void Dispose() => _factory.Dispose();
}
