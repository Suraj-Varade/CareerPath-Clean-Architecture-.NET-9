using Core.Interfaces;
using Core.RequestHelpers;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

//sql
builder.Services.AddDbContext<CareerPathDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ICareerPathRepository, CareerPathRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

try
{
    var scope =  app.Services.CreateScope();
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

app.MapGet("/api/employees",  async ([AsParameters] RequestParams requestParams, ICareerPathRepository repo) =>
{
    var employees = await repo.GetEmployeesAsync(requestParams);
    return Results.Ok(employees);
});

app.MapGet("/api/employees/{id:int}",  async (int id, ICareerPathRepository repo) =>
{
    var employee = await repo.GetEmployeeByIdAsync(id);
    if (employee == null)
    {
        Results.NotFound("employee not found");
    }
    return Results.Ok(employee);
});

app.Run();
