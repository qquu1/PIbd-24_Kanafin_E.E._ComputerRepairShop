using ComputerRepairShop.Data;
using ComputerRepairShop.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ComputerRepairShop.Repositories.Implementations
{
    internal class ClientRepository : IClientRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(DataContext context, ILogger<ClientRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<Client> ReadClients()
        {
            _logger.LogInformation("Чтение списка клиентов");

            var clients = _context.Clients
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .ToList();

            _logger.LogInformation("Прочитано клиентов: {Count}", clients.Count);
            return clients;
        }

        public Client ReadClientById(int id)
        {
            _logger.LogInformation("Чтение клиента с идентификатором {Id}", id);

            var client = _context.Clients
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);

            if (client is null)
            {
                _logger.LogWarning("Клиент с идентификатором {Id} не найден", id);
                throw new InvalidOperationException($"Клиент с идентификатором {id} не найден");
            }

            _logger.LogInformation("Клиент найден: {LastName} {FirstName}", client.LastName, client.FirstName);
            return client;
        }

        public void CreateClient(Client client)
        {
            ArgumentNullException.ThrowIfNull(client);

            _logger.LogInformation("Добавление клиента: {FirstName} {LastName}, телефон {Phone}",
                client.FirstName, client.LastName, client.Phone);

            _context.Clients.Add(client);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Клиент добавлен, идентификатор {Id}", client.Id);
        }

        public void UpdateClient(Client client)
        {
            ArgumentNullException.ThrowIfNull(client);

            _logger.LogInformation("Изменение клиента {Id}: {FirstName} {LastName}, телефон {Phone}",
                client.Id, client.FirstName, client.LastName, client.Phone);

            _context.Clients.Update(client);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Клиент {Id} изменён", client.Id);
        }

        public void DeleteClient(int id)
        {
            _logger.LogInformation("Удаление клиента {Id}", id);

            var client = _context.Clients.FirstOrDefault(c => c.Id == id);
            if (client is null)
            {
                _logger.LogWarning("Клиент с идентификатором {Id} не найден", id);
                throw new InvalidOperationException($"Клиент с идентификатором {id} не найден");
            }

            _context.Clients.Remove(client);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();

            _logger.LogInformation("Клиент {Id} удалён", id);
        }
    }
}
