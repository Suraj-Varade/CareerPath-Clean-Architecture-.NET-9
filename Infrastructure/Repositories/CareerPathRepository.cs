using Core.Entities;
using Core.Interfaces;
using Core.RequestHelpers;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CareerPathRepository : ICareerPathRepository
{
    private readonly CareerPathDbContext context;

    public CareerPathRepository(CareerPathDbContext _context)
    {
        context = _context;
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        await context.Employees.AddAsync(employee);
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        var employee = await context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeCareerHistories)
            .ThenInclude(ch => ch.Role)
            .Include(e => e.EmployeeCareerHistories)
            .ThenInclude(ch => ch.Manager)
            .FirstOrDefaultAsync(e => e.Id == id);

        return employee;
    }

    public async Task<PagedResult<Employee>> GetEmployeesAsync(RequestParams? requestParams)
    {
        requestParams ??= new RequestParams();

        // 1. Eager Load all required relationships (Includes)
        var query = context.Employees
            .AsNoTracking()
            .Include(e => e.EmployeeCareerHistories) // Load career histories
            .ThenInclude(ch => ch.Role) // Load the Role for each history
            .Include(e => e.EmployeeCareerHistories)
            .ThenInclude(ch => ch.Manager) // Load the Manager for each history
            .AsQueryable();

        if (!string.IsNullOrEmpty(requestParams.IsActive))
        {
            switch (requestParams.IsActive.ToLower())
            {
                case "true":
                    query = query.Where(e => e.Status == "Active");
                    break;
                case "false":
                    query = query.Where(e => e.Status == "Inactive");
                    break;
            }
        }
        
        // 2. Apply Ordering (Done in the database)
        query = requestParams.OrderBy?.ToLower() switch
        {
            "joiningdatedesc" => query.OrderByDescending(e => e.DateOfJoining),
            _ => query.OrderBy(e => e.DateOfJoining),
        };
        
        if (!string.IsNullOrWhiteSpace(requestParams.SearchTerm))
        {
            var term = $"%{requestParams.SearchTerm}%";

            query = query.Where(e => EF.Functions.Like(e.Name, term) ||
                                     EF.Functions.Like(e.Email, term) ||
                                     (e.PhoneNumber != null && EF.Functions.Like(e.PhoneNumber, term)));
        }

        /*
        if (!string.IsNullOrWhiteSpace(requestParams.SearchTerm))
        {
            var searchTerm = requestParams.SearchTerm.ToLower();
            query = query.Where(e => e.Name.ToLower().Contains(searchTerm)
                                     || (e.PhoneNumber != null && e.PhoneNumber.ToLower().Contains(searchTerm))
                                     || e.Email.ToLower().Contains(searchTerm));
        }
        */

        var items = await query
            .Skip((requestParams.PageNumber - 1) * requestParams.PageSize)
            .Take(requestParams.PageSize)
            .ToListAsync();
        
        return new PagedResult<Employee>
        {
            TotalCount = await query.CountAsync(),
            Data = items,
        };
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        var roles = await context.Roles.AsNoTracking().ToListAsync();
        return roles;
    }
}