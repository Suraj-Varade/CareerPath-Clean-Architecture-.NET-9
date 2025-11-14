using Core.Entities;
using Core.RequestHelpers;

namespace Core.Interfaces;

public interface ICareerPathRepository
{
    Task AddEmployeeAsync(Employee employee);
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<PagedResult<Employee>> GetEmployeesAsync(RequestParams? requestParams);
    Task<List<Role>> GetRolesAsync();
    Task<bool> SaveChanges();
}