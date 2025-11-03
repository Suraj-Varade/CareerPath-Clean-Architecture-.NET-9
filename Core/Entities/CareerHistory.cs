using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("CareerHistory")]
[Index(nameof(EmployeeId), Name = "IX_CareerHistory_EmployeeId")]
[Index(nameof(ManagerId), Name = "IX_CareerHistory_ManagerId")]
[Index(nameof(RoleId), Name = "IX_CareerHistory_RoleId")]
[Index(nameof(EmployeeId), nameof(StartDate), Name = "IX_CareerHistory_EmployeeId_StartDate")]
public class CareerHistory : BaseEntity
{
    [Required] public int EmployeeId { get; set; }

    [Required] public int RoleId { get; set; }

    public int? ManagerId { get; set; } // nullable, because CEO may not have manager.

    [Required] public string Department { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18, 2)")] public decimal Salary { get; set; }

    [Required] public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; } //nullable - means current position

    [MaxLength(500)] public string Notes { get; set; } = string.Empty;

    //[ForeignKey(nameof(EmployeeId))]
    [JsonIgnore]
    public Employee Employee { get; set; } = null!;
    
    //[ForeignKey(nameof(RoleId))]
    [JsonIgnore]
    public Role Role { get; set; } = null!;
    
    //[ForeignKey(nameof(ManagerId))]
    [JsonIgnore]
    public Employee? Manager { get; set; }

    [NotMapped]
    public int DurationInMonths => EndDate.HasValue
        ? EndDate.Value.Subtract(StartDate).Days / 30
        : DateTime.UtcNow.Subtract(StartDate).Days / 30;
}