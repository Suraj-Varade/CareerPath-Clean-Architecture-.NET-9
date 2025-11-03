using Core.DTOs;
using Core.Entities;
using Core.RequestHelpers;

namespace Core.Interfaces;

public interface ICareerPathRepository
{
    Task AddEmployeeAsync(CreateEmployeeDto employee);
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(RequestParams? requestParams);
    Task<List<RoleDto>> GetRolesAsync();
    Task<bool> SaveChanges();
}