using ComputerRepairShop.Data;
using ComputerRepairShop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComputerRepairShop.Repositories.Implementations
{
    internal class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(DataContext context, ILogger<EmployeeRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Employee> ReadEmployees()
        {
            _logger.LogInformation("Чтение списка работников");

            var employees = _context.Employees
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .ToList();

            _logger.LogInformation("Прочитано работников: {Count}", employees.Count);
            return employees;
        }

        public Employee ReadEmployeeById(int id)
        {
            _logger.LogInformation("Чтение работника с идентификатором {Id}", id);

            var employee = _context.Employees
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == id);

            if (employee is null)
            {
                _logger.LogWarning("Работник с идентификатором {Id} не найден", id);
                throw new InvalidOperationException($"Работник с идентификатором {id} не найден");
            }

            _logger.LogInformation("Работник найден: {LastName} {FirstName}", employee.LastName, employee.FirstName);
            return employee;
        }

        public void CreateEmployee(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee);

            _logger.LogInformation("Добавление работника: {FirstName} {LastName}, должность {Post}",
                employee.FirstName, employee.LastName, employee.EmployeePost);

            _context.Employees.Add(employee);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Работник добавлен, идентификатор {Id}", employee.Id);
        }

        public void UpdateEmployee(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee);

            _logger.LogInformation("Изменение работника {Id}: {FirstName} {LastName}, должность {Post}",
                employee.Id, employee.FirstName, employee.LastName, employee.EmployeePost);

            _context.Employees.Update(employee);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Работник {Id} изменён", employee.Id);
        }

        public void DeleteEmployee(int id)
        {
            _logger.LogInformation("Удаление работника {Id}", id);

            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee is null)
            {
                _logger.LogWarning("Работник с идентификатором {Id} не найден", id);
                throw new InvalidOperationException($"Работник с идентификатором {id} не найден");
            }

            _context.Employees.Remove(employee);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Работник {Id} удалён", id);
        }
    }
}
