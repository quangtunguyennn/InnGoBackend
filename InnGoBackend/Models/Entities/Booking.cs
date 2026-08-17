using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InnGoHotel.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Mối quan hệ
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }

        // Navigation Properties
        public Payment Payment { get; set; } // 1 Booking có 1 Payment
        public ICollection<BookingService> BookingServices { get; set; }
    }
}