using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComputerRepairShop.Entities.Enums;

namespace ComputerRepairShop.Entities
{
    /// <summary>
    /// Заказ на ремонт.
    /// </summary>
    public class RepairOrder
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Column("device_id")]
        public int DeviceId { get; private set; }

        [Browsable(false)]
        [ForeignKey(nameof(DeviceId))]
        public Device? Device { get; private set; }

        [Column("employee_id")]
        public int EmployeeId { get; private set; }

        [Browsable(false)]
        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; private set; }

        [Column("warehouse_id")]
        public int? WarehouseId { get; private set; }

        [Browsable(false)]
        [ForeignKey(nameof(WarehouseId))]
        public Warehouse? Warehouse { get; private set; }

        [Column("order_date", TypeName = "timestamp without time zone")]
        public DateTime OrderDate { get; private set; }

        [Column("repair_status")]
        public RepairStatus RepairStatus { get; private set; }

        [Column("total_cost")]
        public decimal TotalCost { get; private set; }

        /// <summary>
        /// Связь с запчастями через промежуточную таблицу OrderSpareParts.
        /// </summary>
        [Browsable(false)]
        public ICollection<OrderSparePart> OrderSpareParts { get; private set; } = new List<OrderSparePart>();

        private RepairOrder() { }

        public static RepairOrder CreateOperation(int id, int deviceId, int employeeId,
            int? warehouseId, RepairStatus status, decimal totalCost, IEnumerable<OrderSparePart> parts)
        {
            return new RepairOrder
            {
                Id = id,
                DeviceId = deviceId,
                EmployeeId = employeeId,
                WarehouseId = warehouseId,
                OrderDate = DateTime.Now,
                RepairStatus = status,
                TotalCost = totalCost,
                OrderSpareParts = parts?.ToList() ?? new List<OrderSparePart>()
            };
        }
    }
}
