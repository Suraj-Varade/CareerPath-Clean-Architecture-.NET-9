using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("Employee")]
public class Employee : BaseEntity
{
    //personal details
    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    //Employment details
    public string Status { get; set; } = "Active";
    public DateTime DateOfJoining { get; set; } = DateTime.UtcNow;
    public DateTime? DateOfExit { get; set; }

    [NotMapped]
    public int TenureInYears => DateOfExit.HasValue
        ? (DateOfExit.Value - DateOfJoining).Days / 365
        : (DateTime.UtcNow - DateOfJoining).Days / 365;

    public ICollection<CareerHistory> EmployeeCareerHistories { get; set; } = new List<CareerHistory>();
    public ICollection<CareerHistory> ManagerCareerHistories { get; set; } = new List<CareerHistory>();
}