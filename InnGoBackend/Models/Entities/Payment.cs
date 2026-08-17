using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InnGoHotel.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; } // VNPay, Momo, Credit Card, Cash

        public string PaymentStatus { get; set; }

        public DateTime? TransactionDate { get; set; }

        // Mối quan hệ (1-1 với Booking)
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }
    }
}