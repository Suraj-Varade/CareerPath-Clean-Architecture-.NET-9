using System.Globalization;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.RequestHelpers;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CareerPathRepository : ICareerPathRepository
{
    private readonly CareerPathDbContext context;

    public CareerPathRepository(CareerPathDbContext _context)
    {
        context = _context;
    }

    public async Task AddEmployeeAsync(CreateEmployeeDto employee)
    {
        await context.Employees.AddAsync(new Employee
        {
            Name = employee.Name,
            Address = employee.Address,
            PhoneNumber = employee.PhoneNumber,
            Email = employee.Email,
            Status = employee.Status,
            DateOfJoining = DateTime.UtcNow
        });
    }

    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employeeInfo = await context.Employees
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto
            {
                Name = e.Name,
                Address = e.Address,
                PhoneNumber = e.PhoneNumber,
                Email = e.Email,
                Status = e.Status,

                //Formatting Dates
                DateOfJoining = e.DateOfJoining.ToIsoDate(),
                DateOfExit = e.DateOfExit.ToIsoDate(),

                //not Mapped
                TenureInYears = e.TenureInYears,

                //projection related to career history
                CareerDetails = e.EmployeeCareerHistories
                    .OrderByDescending(ch => ch.StartDate)
                    .Select(ch => new CareerHistoryDto
                    {
                        //projecting role details
                        Role = new RoleDto
                        {
                            Title = ch.Role.Title,
                            Level = ch.Role.Level,
                        },
                        ManagerName = ch.Manager!.Name,
                        Department = ch.Department,

                        Salary = ch.Salary.ToString("C", CultureInfo.InvariantCulture), //with Currency

                        //Formatting Dates
                        StartDate = ch.StartDate.ToIsoDate(),
                        EndDate = ch.EndDate.ToIsoDate(),

                        Notes = ch.Notes,
                        DurationInMonths = ch.DurationInMonths
                    })
            }).FirstOrDefaultAsync();
        return employeeInfo;
    }

    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(RequestParams? requestParams)
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

        // 2. Apply Ordering (Done in the database)
        if (!string.IsNullOrWhiteSpace(requestParams.OrderBy))
        {
            // ... your switch statement for OrderBy...
            switch (requestParams.OrderBy.ToLower())
            {
                case "joiningdatedesc":
                    query = query.OrderByDescending(e => e.DateOfJoining);
                    break;
                default:
                    query = query.OrderBy(e => e.DateOfJoining);
                    break;
            }
        }

        if (!string.IsNullOrWhiteSpace(requestParams.SearchTerm))
        {
            var searchTerm = requestParams.SearchTerm.ToLower();
            query = query.Where(e => e.Name.ToLower().Contains(searchTerm)
                                     || (e.PhoneNumber != null && e.PhoneNumber.ToLower().Contains(searchTerm))
                                     || e.Email.ToLower().Contains(searchTerm));
        }


        // 3. Execute the query and switch to in-memory processing (AsEnumerable)
        //    Only retrieve the data types that need formatting (e.g., DateTime)
        var employeeEntities = await query
            .Skip((requestParams.PageNumber - 1) * requestParams.PageSize)
            .Take(requestParams.PageSize)
            .ToListAsync();

        // 4. Project and Format in C# (after ToList/AsEnumerable)
        var employeeDtos = employeeEntities.Select(e => new EmployeeDto
        {
            Name = e.Name,
            Address = e.Address,
            PhoneNumber = e.PhoneNumber,
            Email = e.Email,
            Status = e.Status,

            // Perform C# formatting here
            DateOfJoining = e.DateOfJoining.ToIsoDate(),
            DateOfExit = e.DateOfExit.ToIsoDate(),

            TenureInYears = e.TenureInYears, // This NotMapped property is safe here

            CareerDetails = e.EmployeeCareerHistories
                .OrderByDescending(ch => ch.StartDate) // Ordering is fine in memory
                .Select(ch => new CareerHistoryDto
                {
                    Role = new RoleDto
                    {
                        Title = ch.Role.Title,
                        Level = ch.Role.Level,
                    },

                    // Null-conditional access is safer for Manager
                    ManagerName = ch.Manager?.Name,
                    Department = ch.Department,

                    // Perform C# formatting here
                    Salary = ch.Salary.ToString("C", CultureInfo.InvariantCulture),

                    // Perform C# formatting here
                    StartDate = ch.StartDate.ToIsoDate(),
                    EndDate = ch.EndDate.ToIsoDate(),

                    Notes = ch.Notes,
                    DurationInMonths = ch.DurationInMonths // This NotMapped property is safe here
                })
        }).ToList();

        return new  PagedResult<EmployeeDto>
        {
            TotalCount = await query.CountAsync(),
            Data = employeeDtos,
        };
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        var roles = await context.Roles.Select(x => new RoleDto
        {
            Title = x.Title,
            Level = x.Level
        }).ToListAsync();
        return roles;
    }
    
}