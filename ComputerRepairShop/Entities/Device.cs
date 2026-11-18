using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComputerRepairShop.Entities.Enums;

namespace ComputerRepairShop.Entities
{
    /// <summary>
    /// Устройство, принятое в ремонт.
    /// </summary>
    public class Device
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Column("client_id")]
        public int ClientId { get; private set; }

        [Browsable(false)]
        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; private set; }

        [Column("device_type")]
        public DeviceType DeviceType { get; private set; }

        [Required]
        [MaxLength(100)]
        [Column("model")]
        public string Model { get; private set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("serial_number")]
        public string SerialNumber { get; private set; } = string.Empty;

        /// <summary>
        /// Одно устройство — много заказов на ремонт.
        /// </summary>
        [Browsable(false)]
        public ICollection<RepairOrder> RepairOrders { get; private set; } = new List<RepairOrder>();

        private Device() { }

        public static Device CreateEntity(int id, int clientId, DeviceType deviceType,
            string model, string serialNumber)
        {
            return new Device
            {
                Id = id,
                ClientId = clientId,
                DeviceType = deviceType,
                Model = model ?? string.Empty,
                SerialNumber = serialNumber ?? string.Empty
            };
        }
    }
}
