using ComputerRepairShop.Data;
using ComputerRepairShop.Forms;
using ComputerRepairShop.Repositories;
using ComputerRepairShop.Repositories.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Unity;
using Unity.Lifetime;
using Unity.Microsoft.Logging;

namespace ComputerRepairShop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var loggerFactory = CreateLoggerFactory();

            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
                Log.Fatal(args.ExceptionObject as Exception, "Необработанное исключение");

            Log.Information("Запуск приложения учёта ремонта компьютерной техники");

            try
            {
                var container = CreateContainer(loggerFactory);
                Application.Run(container.Resolve<FormMain>());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Критическая ошибка при запуске приложения");
                MessageBox.Show(ex.Message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Log.Information("Завершение работы приложения");
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Настройка IoC-контейнера: DataContext, репозитории и логирование.
        /// </summary>
        private static IUnityContainer CreateContainer(ILoggerFactory loggerFactory)
        {
            var container = new UnityContainer();

            container.AddExtension(new LoggingExtension(loggerFactory));

            container.RegisterType<DataContext>(new TransientLifetimeManager());
            // Репозитории.
            container.RegisterType<IClientRepository, ClientRepository>(new TransientLifetimeManager());
            container.RegisterType<IDeviceRepository, DeviceRepository>(new TransientLifetimeManager());
            container.RegisterType<IEmployeeRepository, EmployeeRepository>(new TransientLifetimeManager());
            container.RegisterType<ISparePartRepository, SparePartRepository>(new TransientLifetimeManager());
            container.RegisterType<IRepairOrderRepository, RepairOrderRepository>(new TransientLifetimeManager());
            container.RegisterType<IWarehouseRepository, WarehouseRepository>(new TransientLifetimeManager());

            return container;
        }

        /// <summary>
        /// Создание фабрики логгеров Serilog по настройкам из appsettings.json.
        /// </summary>
        private static ILoggerFactory CreateLoggerFactory()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(Log.Logger);

            return loggerFactory;
        }
    }
}
