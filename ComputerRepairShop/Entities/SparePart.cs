using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComputerRepairShop.Entities.Enums;

namespace ComputerRepairShop.Entities
{
    /// <summary>
    /// Запчасть (комплектующий).
    /// </summary>
    public class SparePart
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; private set; } = string.Empty;

        [Column("price")]
        public decimal Price { get; private set; }

        [Column("part_category")]
        public PartCategory PartCategory { get; private set; }

        /// <summary>
        /// Связь с заказами через промежуточную таблицу OrderSpareParts.
        /// </summary>
        [Browsable(false)]
        public ICollection<OrderSparePart> OrderSpareParts { get; private set; } = new List<OrderSparePart>();

        private SparePart() { }

        public static SparePart CreateEntity(int id, string name, decimal price, PartCategory category)
        {
            return new SparePart
            {
                Id = id,
                Name = name ?? string.Empty,
                Price = price,
                PartCategory = category
            };
        }
    }
}
