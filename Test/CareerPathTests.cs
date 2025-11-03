using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.RequestHelpers;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;

namespace Test;

[Trait("Category", "BasicRepositoryTest")]
public class CareerPathTests
{
    private CareerPathDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CareerPathDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CareerPathDbContext(options);
    }

    //Repository-Test
    [Fact]
    public async Task AddEmployee_ShouldCreateEmployeeRecord()
    {
        var context = CreateDbContext();
        var repo = new CareerPathRepository(context);
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "John Doe",
            Address = "123 Main St",
            PhoneNumber = "12345678",
            Email = "johndoe@example.com",
            Status = "Active"
        });

        await repo.SaveChanges();

        //get and see if employee present.
        var employee = await repo.GetEmployeeByIdAsync(1);
        Assert.NotNull(employee);
        Assert.Equal("John Doe", employee.Name);
    }

    [Fact]
    public async Task AddEmployee_SaveFail_NoEmployeeRecordCreated()
    {
        var context = CreateDbContext();
        var repo = new CareerPathRepository(context);

        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "John Doe",
            Address = "123 Main St",
            PhoneNumber = "12345678",
            Email = "johndoe@example.com",
            Status = "Active"
        });

        //skipping the saveChanges()
        
        //get and see if employee present.
        var employee = await repo.GetEmployeeByIdAsync(1);
        Assert.Null(employee);
    }

    [Fact]
    public async Task GetEmployeeById_ShouldReturnEmployee()
    {
        var context = CreateDbContext();
        var repo = new CareerPathRepository(context);
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "John Doe",
            Address = "123 Main St",
            PhoneNumber = "12345678",
            Email = "johndoe@example.com",
            Status = "Active"
        });
        await repo.SaveChanges();

        var employee = await repo.GetEmployeeByIdAsync(1);
        Assert.NotNull(employee);
        Assert.Equal("John Doe", employee.Name);
    }

    [Fact]
    //pageSize = 2
    public async Task GetAllEmployees_ShouldReturnTwoEmployeeRecords()
    {
        var context = CreateDbContext();
        var repo = new CareerPathRepository(context);
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "John Doe",
            Address = "123 Main St",
            PhoneNumber = "12345678",
            Email = "johndoe@example.com",
            
            Status = "Active"
        });
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "rocky",
            Address = "side road 234 st",
            PhoneNumber = "888888888",
            Email = "rocky@example.com",
            Status = "Active"
        });
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "Steve Smith",
            Address = "-",
            PhoneNumber = "83462309123",
            Email = "stevesmith@example.com",
            Status = "Active"
        });
        await repo.SaveChanges();

        var employee = await repo.GetEmployeesAsync(new RequestParams()
        {
            PageNumber = 1,
            PageSize = 2,
            OrderBy = "joiningdatedesc"
        });

        Assert.NotNull(employee);
        Assert.Equal(3, employee.TotalCount);
        Assert.Equal("Steve Smith", employee.Data.First().Name);
    }

    [Fact]
    public async Task GetAllEmployees_WithSearchTerm_ShouldReturnCorrectEmployeeRecords()
    {
        var context = CreateDbContext();
        var repo = new CareerPathRepository(context);
        
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "John Doe",
            Address = "123 Main St",
            PhoneNumber = "12345678",
            Email = "johndoe@example.com",
            
            Status = "Active"
        });
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "rocky",
            Address = "side road 234 st",
            PhoneNumber = "888888888",
            Email = "rocky@example.com",
            Status = "Active"
        });
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "Steve Smith",
            Address = "-",
            PhoneNumber = "83462309123",
            Email = "stevesmith@example.com",
            Status = "Active"
        });
        
        await repo.SaveChanges();

        var employees = await repo.GetEmployeesAsync(new RequestParams()
        {
            PageNumber = 1,
            PageSize = 5,
            SearchTerm = "rocky"
        });
        
        Assert.NotNull(employees);
        Assert.Single(employees.Data);
        Assert.NotEqual("Steve Smith", employees.Data.First().Name);
        Assert.Equal("rocky", employees.Data.First().Name);
    }
    
    [Fact]
    public async Task GetAllEmployees_WithNoRequestParams_ShouldReturnEmployeeRecordsBasedOnDefaultConfiguration()
    {
        var context = CreateDbContext();
        var repo = new CareerPathRepository(context);
        
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "John Doe",
            Address = "123 Main St",
            PhoneNumber = "12345678",
            Email = "johndoe@example.com",
            
            Status = "Active"
        });
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "rocky",
            Address = "side road 234 st",
            PhoneNumber = "888888888",
            Email = "rocky@example.com",
            Status = "Active"
        });
        await repo.AddEmployeeAsync(new CreateEmployeeDto
        {
            Name = "Steve Smith",
            Address = "-",
            PhoneNumber = "83462309123",
            Email = "stevesmith@example.com",
            Status = "Active"
        });
        
        await repo.SaveChanges();

        var employees = await repo.GetEmployeesAsync(null);

        Assert.NotNull(employees);
        Assert.Equal(3, employees.TotalCount);
    }
    
    //test Employee Seed
    [Fact]
    public async Task PerformDataSeed_ShouldSeedEmployees()
    {
        var context = CreateDbContext();
        await CareerPathContextSeed.SeedAsync(context);
        var repo = new CareerPathRepository(context);
        var seededEmployees = await repo.GetEmployeesAsync(new RequestParams()
        {
            PageNumber = 1,
            PageSize = 20,
        });
        Assert.NotNull(seededEmployees);
        Assert.Equal(10, seededEmployees.TotalCount);
        
        var seededRoles = await repo.GetRolesAsync();
        Assert.NotNull(seededRoles);
        Assert.Equal(8, seededRoles.Count());
    }
}