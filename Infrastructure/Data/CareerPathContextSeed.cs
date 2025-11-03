using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class CareerPathContextSeed
{
    public static async Task SeedAsync(CareerPathDbContext context)
    {
        if (!context.Employees.Any())
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "Data", "SeedData", "Employees.json");
            var employeeData = await File.ReadAllTextAsync(filePath);
            var employees = JsonSerializer.Deserialize<List<Employee>>(employeeData);
            if (employees == null)
            {
                return;
            }
            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();
        }
        
        if (!context.Roles.Any())
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "Data", "SeedData", "Roles.json");
            var roleData = await File.ReadAllTextAsync(filePath);
            var roles = JsonSerializer.Deserialize<List<Role>>(roleData);
            if (roles == null)
            {
                return;
            }
            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }
        
        if (!context.CareerHistories.Any())
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "Data", "SeedData", "CareerHistory.json");
            var careerHistoryData = await File.ReadAllTextAsync(filePath);
            var careerHistories = JsonSerializer.Deserialize<List<CareerHistory>>(careerHistoryData);
            if (careerHistories == null)
            {
                return;
            }
            context.CareerHistories.AddRange(careerHistories);
            await context.SaveChangesAsync();
        }
    }
}