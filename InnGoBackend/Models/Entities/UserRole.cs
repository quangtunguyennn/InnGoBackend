using InnGoHotel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace InnGoBackend.Models
{
    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; } // Khóa chính mới của bảng

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}