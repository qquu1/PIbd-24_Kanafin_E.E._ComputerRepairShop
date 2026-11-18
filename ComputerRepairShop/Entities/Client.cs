using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerRepairShop.Entities
{
    /// <summary>
    /// Клиент сервисного центра.
    /// </summary>
    public class Client
    {
        [Key]
        [Column("id")]
        public int Id { get; private set; }

        [Required]
        [MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; private set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("last_name")]
        public string LastName { get; private set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("phone")]
        public string Phone { get; private set; } = string.Empty;

        /// <summary>
        /// Один клиент — много устройств (связь один-ко-многим).
        /// </summary>
        [Browsable(false)]
        public ICollection<Device> Devices { get; private set; } = new List<Device>();

        private Client() { }

        public static Client CreateEntity(int id, string firstName, string lastName, string phone)
        {
            return new Client
            {
                Id = id,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Phone = phone ?? string.Empty
            };
        }
    }
}
