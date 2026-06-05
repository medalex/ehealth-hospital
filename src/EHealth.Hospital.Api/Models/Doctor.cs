namespace EHealth.Hospital.Models;

public class Doctor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Specialty { get; set; } = default!;
    public string LicenseNumber { get; set; } = default!;
    public string? CredentialUal { get; set; }
}
