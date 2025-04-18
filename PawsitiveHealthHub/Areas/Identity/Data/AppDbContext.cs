using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PawsitiveHealthHub.Areas.Identity.Data;
using PawsitiveHealthHub.Models;

namespace PawsitiveHealthHub.Areas.Identity.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Add DbSet properties for your domain models
    public DbSet<Pets> Pets { get; set; }
    public DbSet<Appointments> Appointments { get; set; }
    public DbSet<MedRecords> MedRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configuration for ApplicationUser
        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

        // Configure relationships for Appointment
        builder.Entity<Appointments>()
            .HasOne(a => a.Owner)
            .WithMany(u => u.OwnerAppointments)
            .HasForeignKey(a => a.OwnerID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointments>()
            .HasOne(a => a.Vet)
            .WithMany(u => u.VetAppointments)
            .HasForeignKey(a => a.VetID)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship for MedRecord
        builder.Entity<MedRecords>()
            .HasOne(m => m.Vet)
            .WithMany(u => u.VetMedRecords)
            .HasForeignKey(m => m.VetID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(20);
        builder.Property(u => u.LastName).HasMaxLength(20);

        // Configure navigation properties
        builder.HasMany(u => u.Pets)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.OwnerAppointments)
            .WithOne(a => a.Owner)
            .HasForeignKey(a => a.OwnerID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.VetAppointments)
            .WithOne(a => a.Vet)
            .HasForeignKey(a => a.VetID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.VetMedRecords)
            .WithOne(m => m.Vet)
            .HasForeignKey(m => m.VetID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

