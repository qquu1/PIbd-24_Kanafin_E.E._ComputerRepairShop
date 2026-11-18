using ComputerRepairShop.Data;
using ComputerRepairShop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComputerRepairShop.Repositories.Implementations
{
    internal class DeviceRepository : IDeviceRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<DeviceRepository> _logger;

        public DeviceRepository(DataContext context, ILogger<DeviceRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Device> ReadDevices()
        {
            _logger.LogInformation("Чтение списка устройств");

            var devices = _context.Devices
                .AsNoTracking()
                .OrderBy(d => d.Id)
                .ToList();

            _logger.LogInformation("Прочитано устройств: {Count}", devices.Count);
            return devices;
        }

        public Device ReadDeviceById(int id)
        {
            _logger.LogInformation("Чтение устройства с идентификатором {Id}", id);

            var device = _context.Devices
                .AsNoTracking()
                .FirstOrDefault(d => d.Id == id);

            if (device is null)
            {
                _logger.LogWarning("Устройство с идентификатором {Id} не найдено", id);
                throw new InvalidOperationException($"Устройство с идентификатором {id} не найдено");
            }

            _logger.LogInformation("Устройство найдено: {Model}, серийный номер {SerialNumber}",
                device.Model, device.SerialNumber);
            return device;
        }

        public void CreateDevice(Device device)
        {
            ArgumentNullException.ThrowIfNull(device);

            _logger.LogInformation("Добавление устройства: {Model}, тип {DeviceType}, клиент {ClientId}",
                device.Model, device.DeviceType, device.ClientId);

            _context.Devices.Add(device);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Устройство добавлено, идентификатор {Id}", device.Id);
        }

        public void UpdateDevice(Device device)
        {
            ArgumentNullException.ThrowIfNull(device);

            _logger.LogInformation("Изменение устройства {Id}: {Model}, тип {DeviceType}, клиент {ClientId}",
                device.Id, device.Model, device.DeviceType, device.ClientId);

            _context.Devices.Update(device);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Устройство {Id} изменено", device.Id);
        }

        public void DeleteDevice(int id)
        {
            _logger.LogInformation("Удаление устройства {Id}", id);

            var device = _context.Devices.FirstOrDefault(d => d.Id == id);
            if (device is null)
            {
                _logger.LogWarning("Устройство с идентификатором {Id} не найдено", id);
                throw new InvalidOperationException($"Устройство с идентификатором {id} не найдено");
            }

            _context.Devices.Remove(device);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Устройство {Id} удалено", id);
        }
    }
}
