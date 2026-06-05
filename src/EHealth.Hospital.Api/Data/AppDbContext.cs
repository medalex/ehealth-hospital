using EHealth.Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace EHealth.Hospital.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<AllergyRecord> AllergyRecords => Set<AllergyRecord>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();

}
