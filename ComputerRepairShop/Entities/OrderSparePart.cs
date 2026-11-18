using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerRepairShop.Entities
{
    /// <summary>
    /// Промежуточная таблица связи «многие-ко-многим» между заказом на ремонт и запчастями.
    /// </summary>
    public class OrderSparePart
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Column("repair_order_id")]
        public int RepairOrderId { get; private set; }

        [Browsable(false)]
        [ForeignKey(nameof(RepairOrderId))]
        public RepairOrder? RepairOrder { get; private set; }

        [Column("spare_part_id")]
        public int SparePartId { get; private set; }

        [Browsable(false)]
        [ForeignKey(nameof(SparePartId))]
        public SparePart? SparePart { get; private set; }

        [Column("count")]
        public int Count { get; private set; }

        private OrderSparePart() { }

        public static OrderSparePart CreateElement(int id, int sparePartId, int count)
        {
            return new OrderSparePart
            {
                Id = id,
                SparePartId = sparePartId,
                Count = count
            };
        }
    }
}
