using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerRepairShop.Entities
{
    /// <summary>
    /// Склад.
    /// </summary>
    public class Warehouse
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; private set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("address")]
        public string Address { get; private set; } = string.Empty;

        [Column("capacity")]
        public int Capacity { get; private set; }

        [Column("occupied_places")]
        public int OccupiedPlaces { get; private set; }

        [NotMapped]
        public int AvailablePlaces => Capacity - OccupiedPlaces;

        /// <summary>
        /// Один склад — много заказов на ремонт.
        /// </summary>
        [Browsable(false)]
        public ICollection<RepairOrder> RepairOrders { get; private set; } = new List<RepairOrder>();

        private Warehouse() { }

        public static Warehouse CreateEntity(int id, string name, string address,
            int capacity, int occupiedPlaces)
        {
            return new Warehouse
            {
                Id = id,
                Name = name ?? string.Empty,
                Address = address ?? string.Empty,
                Capacity = capacity,
                OccupiedPlaces = occupiedPlaces
            };
        }
    }
}
