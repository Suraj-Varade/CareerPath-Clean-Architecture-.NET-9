using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Core.Entities;

[Table("Role")]
[Index(nameof(Title), IsUnique = true)]
public class Role : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Level { get; set; } = string.Empty; // Junior, Mid, Senior, Lead, Principal, Architect
    
    public int HierarchyLevel { get; set; } //1-Junior, 2-Mid, 3-Senior
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [JsonIgnore]
    public ICollection<CareerHistory> RoleHistories { get; set; } = new List<CareerHistory>();
}