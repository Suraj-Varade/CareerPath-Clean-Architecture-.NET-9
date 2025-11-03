using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class CareerPathDbContext(DbContextOptions<CareerPathDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<CareerHistory> CareerHistories { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //CareerHistory -> Employee (Main)
        modelBuilder.Entity<CareerHistory>()
            .HasOne(ch => ch.Employee)
            .WithMany(e => e.EmployeeCareerHistories)
            .HasForeignKey(ch => ch.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        //CareerHistory -> Manager (also Employee)
        modelBuilder.Entity<CareerHistory>()
            .HasOne(ch => ch.Manager)
            .WithMany(m => m.ManagerCareerHistories)
            .HasForeignKey(ch => ch.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        //CareerHistory -> Role
        modelBuilder.Entity<CareerHistory>()
            .HasOne(ch => ch.Role)
            .WithMany(r => r.RoleHistories)
            .HasForeignKey(ch => ch.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}