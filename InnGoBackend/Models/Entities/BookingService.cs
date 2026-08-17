using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InnGoHotel.Models
{
    public class BookingService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingServiceId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Lưu lại giá dịch vụ tại thời điểm đặt

        // Mối quan hệ
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }

        [ForeignKey("ServiceId")]
        public Service Service { get; set; }
    }
}