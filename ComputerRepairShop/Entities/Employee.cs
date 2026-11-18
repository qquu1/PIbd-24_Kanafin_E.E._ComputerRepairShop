using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComputerRepairShop.Entities.Enums;

namespace ComputerRepairShop.Entities
{
    /// <summary>
    /// Работник сервисного центра.
    /// </summary>
    public class Employee
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

        [Column("employee_post")]
        public EmployeePost EmployeePost { get; private set; }

        /// <summary>
        /// Один работник — много заказов на ремонт.
        /// </summary>
        [Browsable(false)]
        public ICollection<RepairOrder> RepairOrders { get; private set; } = new List<RepairOrder>();

        private Employee() { }

        public static Employee CreateEntity(int id, string firstName, string lastName, EmployeePost post)
        {
            return new Employee
            {
                Id = id,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                EmployeePost = post
            };
        }
    }
}
