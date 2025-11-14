namespace API.DTOs;

public class EmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string Email { get; set; } = string.Empty;

    // Employment Details
    public string Status { get; set; } = string.Empty;
    public string DateOfJoining { get; set; } = string.Empty; // Format as readable string
    public string? DateOfExit { get; set; } // Format as readable string
    public int TenureInYears { get; set; } // Keep as int

    // Related data
    public IEnumerable<CareerHistoryDto> CareerDetails { get; set; } = new List<CareerHistoryDto>();
}