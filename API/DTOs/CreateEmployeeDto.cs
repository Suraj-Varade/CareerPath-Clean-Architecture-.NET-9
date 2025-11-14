using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateEmployeeDto
{
    [Required] public string Name { get; set; } = String.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";
    
    public DateTime DateOfJoining { get; set; }
}