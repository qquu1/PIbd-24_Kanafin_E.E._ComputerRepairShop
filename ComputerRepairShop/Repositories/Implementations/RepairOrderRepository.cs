using ComputerRepairShop.Data;
using ComputerRepairShop.Entities;
using ComputerRepairShop.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComputerRepairShop.Repositories.Implementations
{
    internal class RepairOrderRepository : IRepairOrderRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<RepairOrderRepository> _logger;

        public RepairOrderRepository(DataContext context, ILogger<RepairOrderRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<RepairOrder> ReadRepairOrders(DateTime? dateFrom = null, DateTime? dateTo = null,
            int? employeeId = null, RepairStatus? status = null)
        {
            _logger.LogInformation(
                "Чтение заказов на ремонт: дата с {DateFrom}, по {DateTo}, работник {EmployeeId}, статус {Status}",
                dateFrom, dateTo, employeeId, status);

            var query = _context.RepairOrders
                .AsNoTracking()
                .Include(r => r.Device)
                .Include(r => r.Employee)
                .Include(r => r.Warehouse)
                .Include(r => r.OrderSpareParts)
                    .ThenInclude(p => p.SparePart)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                query = query.Where(r => r.OrderDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(r => r.OrderDate <= dateTo.Value);
            }

            if (employeeId.HasValue)
            {
                query = query.Where(r => r.EmployeeId == employeeId.Value);
            }

            if (status.HasValue)
            {
                var mask = (int)status.Value;
                query = query.Where(r => ((int)r.RepairStatus & mask) == mask);
            }

            var orders = query
                .OrderByDescending(r => r.OrderDate)
                .ThenByDescending(r => r.Id)
                .ToList();

            _logger.LogInformation("Прочитано заказов: {Count}", orders.Count);
            return orders;
        }

        /// <summary>
        /// Создание заказа вместе со списком запчастей (связь «многие-ко-многим»)
        /// выполняется в одной транзакции.
        /// </summary>
        public void CreateRepairOrder(RepairOrder order)
        {
            ArgumentNullException.ThrowIfNull(order);

            _logger.LogInformation(
                "Добавление заказа на ремонт: устройство {DeviceId}, работник {EmployeeId}, склад {WarehouseId}, " +
                "статус {Status}, стоимость {TotalCost}, запчастей {PartsCount}",
                order.DeviceId, order.EmployeeId, order.WarehouseId, order.RepairStatus,
                order.TotalCost, order.OrderSpareParts.Count);

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.RepairOrders.Add(order);
                _context.SaveChanges();
                _context.ChangeTracker.Clear();

                transaction.Commit();

                _logger.LogInformation("Заказ на ремонт добавлен, идентификатор {Id}", order.Id);
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                _logger.LogError(ex, "Ошибка при добавлении заказа на ремонт, транзакция отменена");
                throw;
            }
        }

        public void DeleteRepairOrder(int id)
        {
            _logger.LogInformation("Удаление заказа на ремонт {Id}", id);

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var order = _context.RepairOrders
                    .Include(r => r.OrderSpareParts)
                    .FirstOrDefault(r => r.Id == id);

                if (order is null)
                {
                    _logger.LogWarning("Заказ на ремонт с идентификатором {Id} не найден", id);
                    throw new InvalidOperationException($"Заказ на ремонт с идентификатором {id} не найден");
                }

                _context.RepairOrders.Remove(order);
                _context.SaveChanges();
                _context.ChangeTracker.Clear();

                transaction.Commit();

                _logger.LogInformation("Заказ на ремонт {Id} удалён вместе со связанными запчастями ({PartsCount})",
                    id, order.OrderSpareParts.Count);
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                _logger.LogError(ex, "Ошибка при удалении заказа на ремонт {Id}, транзакция отменена", id);
                throw;
            }
        }
    }
}
