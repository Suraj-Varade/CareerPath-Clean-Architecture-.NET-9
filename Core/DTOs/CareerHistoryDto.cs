namespace Core.DTOs;

public class CareerHistoryDto
{
    // Include the Role details directly for context
    public RoleDto Role { get; set; } = new RoleDto();

    // Include Manager Name instead of just ManagerId
    public string? ManagerName { get; set; } 

    public string Department { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty; // Format as a currency string
    public string StartDate { get; set; } = string.Empty; // Format as readable string
    public string? EndDate { get; set; } // Format as readable string
    public string Notes { get; set; } = string.Empty;
    public int DurationInMonths { get; set; }
}