using InnGoBackend.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InnGoHotel.Models
{
    public class Wishlist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WishlistId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BranchId { get; set; }
        [Required]
        public int RoomId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Mối quan hệ
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
    }
}