using ComputerRepairShop.Data;
using ComputerRepairShop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComputerRepairShop.Repositories.Implementations
{
    internal class SparePartRepository : ISparePartRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<SparePartRepository> _logger;

        public SparePartRepository(DataContext context, ILogger<SparePartRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<SparePart> ReadSpareParts()
        {
            _logger.LogInformation("Чтение списка запчастей");

            var spareParts = _context.SpareParts
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .ToList();

            _logger.LogInformation("Прочитано запчастей: {Count}", spareParts.Count);
            return spareParts;
        }

        public SparePart ReadSparePartById(int id)
        {
            _logger.LogInformation("Чтение запчасти с идентификатором {Id}", id);

            var sparePart = _context.SpareParts
                .AsNoTracking()
                .FirstOrDefault(s => s.Id == id);

            if (sparePart is null)
            {
                _logger.LogWarning("Запчасть с идентификатором {Id} не найдена", id);
                throw new InvalidOperationException($"Запчасть с идентификатором {id} не найдена");
            }

            _logger.LogInformation("Запчасть найдена: {Name}, цена {Price}", sparePart.Name, sparePart.Price);
            return sparePart;
        }

        public void CreateSparePart(SparePart sparePart)
        {
            ArgumentNullException.ThrowIfNull(sparePart);

            _logger.LogInformation("Добавление запчасти: {Name}, цена {Price}, категории {Category}",
                sparePart.Name, sparePart.Price, sparePart.PartCategory);

            _context.SpareParts.Add(sparePart);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Запчасть добавлена, идентификатор {Id}", sparePart.Id);
        }

        public void UpdateSparePart(SparePart sparePart)
        {
            ArgumentNullException.ThrowIfNull(sparePart);

            _logger.LogInformation("Изменение запчасти {Id}: {Name}, цена {Price}, категории {Category}",
                sparePart.Id, sparePart.Name, sparePart.Price, sparePart.PartCategory);

            _context.SpareParts.Update(sparePart);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Запчасть {Id} изменена", sparePart.Id);
        }

        public void DeleteSparePart(int id)
        {
            _logger.LogInformation("Удаление запчасти {Id}", id);

            var sparePart = _context.SpareParts.FirstOrDefault(s => s.Id == id);
            if (sparePart is null)
            {
                _logger.LogWarning("Запчасть с идентификатором {Id} не найдена", id);
                throw new InvalidOperationException($"Запчасть с идентификатором {id} не найдена");
            }

            _context.SpareParts.Remove(sparePart);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Запчасть {Id} удалена", id);
        }
    }
}
