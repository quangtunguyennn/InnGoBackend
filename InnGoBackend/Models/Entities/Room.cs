using InnGoBackend.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InnGoHotel.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required, MaxLength(50)]
        public string RoomType { get; set; } // Standard, Deluxe, Suite

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Capacity { get; set; }
        public string Status { get; set; } = "Available"; // Available, Booked, Maintenance
        public string ImageUrl { get; set; }

        // Mối quan hệ với Branch
        [ForeignKey("BranchId")]
        [JsonIgnore]
        public Branch? Branch { get; set; }

        // Navigation Properties (1-N)
        [JsonIgnore]
        public ICollection<Booking>? Bookings { get; set; }
    }
}