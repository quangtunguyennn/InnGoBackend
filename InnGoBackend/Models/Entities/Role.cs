using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InnGoBackend.Models // Thay bằng namespace của bạn
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, MaxLength(50)]
        public string RoleName { get; set; } // Ví dụ: Admin, Customer, Manager

        [MaxLength(255)]
        public string Description { get; set; } // Mô tả vai trò

        // Navigation Properties (1-N)
        public ICollection<UserRole> UserRoles { get; set; }
    }
}