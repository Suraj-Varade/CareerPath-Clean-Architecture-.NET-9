using System.Globalization;
using API.DTOs;
using API.Extensions;
using API.Middlewares;
using Core.Entities;
using Core.Interfaces;
using Core.RequestHelpers;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration); //add logging

builder.Services.AddApplicationServices(builder.Configuration); //add application services

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

if (!app.Environment.IsEnvironment("Testing"))
{
    try
    {
        var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<CareerPathDbContext>();
        await context.Database.MigrateAsync();

        // SeedData
        await CareerPathContextSeed.SeedAsync(context);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}

app.MapGet("/api/employees", async ([AsParameters] RequestParams requestParams, ICareerPathRepository repo) =>
{
    var result = await repo.GetEmployeesAsync(requestParams);

    var dto = new PagedResult<EmployeeDto>()
    {
        TotalCount = result.TotalCount,
        Data = result.Data.Select(e => new EmployeeDto
        {
            Name = e.Name,
            Address = e.Address,
            PhoneNumber = e.PhoneNumber,
            Email = e.Email,
            Status = e.Status,

            // Perform C# formatting here
            DateOfJoining = e.DateOfJoining.ToIsoDate(),
            DateOfExit = e.DateOfExit.ToIsoDate(),

            TenureInYears = e.TenureInYears,

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
                    DurationInMonths = ch.DurationInMonths
                }).ToList()
        }).ToList()
    };

    return Results.Ok(dto);
});

app.MapGet("/api/employees/{id:int}", async (int id, ICareerPathRepository repo) =>
{
    var e = await repo.GetEmployeeByIdAsync(id);
    if (e == null)
    {
        return Results.NotFound("employee not found");
    }

    var employeeDto = new EmployeeDto()
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
                ManagerName = ch.Manager != null ? ch.Manager.Name : null,
                Department = ch.Department,

                Salary = ch.Salary.ToString("C", CultureInfo.InvariantCulture), //with Currency

                //Formatting Dates
                StartDate = ch.StartDate.ToIsoDate(),
                EndDate = ch.EndDate.ToIsoDate(),

                Notes = ch.Notes,
                DurationInMonths = ch.DurationInMonths
            })
    };
    return Results.Ok(employeeDto);
});

app.MapPost("/api/employees", async (CreateEmployeeDto dto, ICareerPathRepository repo) =>
{
    var employee = new Employee()
    {
        Name = dto.Name,
        Address = dto.Address,
        PhoneNumber = dto.PhoneNumber,
        Email = dto.Email,
        Status = dto.Status
    };

    await repo.AddEmployeeAsync(employee);

    if (await repo.SaveChanges())
    {
        return Results.Created($"/api/employees/{employee.Id}", new EmployeeDto()
        {
            Name = employee.Name,
            Address = employee.Address,
            PhoneNumber = employee.PhoneNumber,
            Email = employee.Email,
            Status = employee.Status,
            DateOfJoining = employee.DateOfJoining.ToIsoDate(),
            DateOfExit = employee.DateOfExit.ToIsoDate(),
            TenureInYears = employee.TenureInYears
        });
    }

    return Results.BadRequest("problem adding new employee");
});

app.MapGet("/api/roles", async (ICareerPathRepository repo) =>
{
    var roles = await repo.GetRolesAsync();
    return Results.Ok(roles);
});

await app.RunAsync();