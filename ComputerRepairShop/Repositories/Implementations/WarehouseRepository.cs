using ComputerRepairShop.Data;
using ComputerRepairShop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComputerRepairShop.Repositories.Implementations
{
    internal class WarehouseRepository : IWarehouseRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<WarehouseRepository> _logger;

        public WarehouseRepository(DataContext context, ILogger<WarehouseRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Warehouse> ReadWarehouses()
        {
            _logger.LogInformation("Чтение списка складов");

            var warehouses = _context.Warehouses
                .AsNoTracking()
                .OrderBy(w => w.Id)
                .ToList();

            _logger.LogInformation("Прочитано складов: {Count}", warehouses.Count);
            return warehouses;
        }

        public Warehouse ReadWarehouseById(int id)
        {
            _logger.LogInformation("Чтение склада с идентификатором {Id}", id);

            var warehouse = _context.Warehouses
                .AsNoTracking()
                .FirstOrDefault(w => w.Id == id);

            if (warehouse is null)
            {
                _logger.LogWarning("Склад с идентификатором {Id} не найден", id);
                throw new InvalidOperationException($"Склад с идентификатором {id} не найден");
            }

            _logger.LogInformation("Склад найден: {Name}, свободных мест {AvailablePlaces}",
                warehouse.Name, warehouse.AvailablePlaces);
            return warehouse;
        }

        public void CreateWarehouse(Warehouse warehouse)
        {
            ArgumentNullException.ThrowIfNull(warehouse);

            _logger.LogInformation("Добавление склада: {Name}, адрес {Address}, мест {Capacity}",
                warehouse.Name, warehouse.Address, warehouse.Capacity);

            if (warehouse.OccupiedPlaces > warehouse.Capacity)
            {
                _logger.LogWarning("Некорректные данные склада: занято {Occupied} больше, чем всего {Capacity}",
                    warehouse.OccupiedPlaces, warehouse.Capacity);
                throw new InvalidOperationException("Занятых мест не может быть больше общего количества");
            }

            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Склад добавлен, идентификатор {Id}", warehouse.Id);
        }

        public void UpdateWarehouse(Warehouse warehouse)
        {
            ArgumentNullException.ThrowIfNull(warehouse);

            _logger.LogInformation("Изменение склада {Id}: {Name}, адрес {Address}, занято {Occupied} из {Capacity}",
                warehouse.Id, warehouse.Name, warehouse.Address, warehouse.OccupiedPlaces, warehouse.Capacity);

            if (warehouse.OccupiedPlaces > warehouse.Capacity)
            {
                _logger.LogWarning("Некорректные данные склада {Id}: занято {Occupied} больше, чем всего {Capacity}",
                    warehouse.Id, warehouse.OccupiedPlaces, warehouse.Capacity);
                throw new InvalidOperationException("Занятых мест не может быть больше общего количества");
            }

            _context.Warehouses.Update(warehouse);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Склад {Id} изменён", warehouse.Id);
        }

        public void DeleteWarehouse(int id)
        {
            _logger.LogInformation("Удаление склада {Id}", id);

            var warehouse = _context.Warehouses.FirstOrDefault(w => w.Id == id);
            if (warehouse is null)
            {
                _logger.LogWarning("Склад с идентификатором {Id} не найден", id);
                throw new InvalidOperationException($"Склад с идентификатором {id} не найден");
            }

            _context.Warehouses.Remove(warehouse);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Склад {Id} удалён", id);
        }
    }
}
