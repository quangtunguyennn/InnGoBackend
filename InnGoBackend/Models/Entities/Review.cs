using InnGoBackend.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InnGoHotel.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        [Required]
        public int BookingId { get; set; }
        [JsonIgnore]
        public Booking? Booking { get; set; }
        [Required]
        public int RoomId { get; set; }
        [JsonIgnore]
        public Room? Room { get; set; }

        [Required]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // Đánh giá 1 - 5 sao

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

     

      
    }
}