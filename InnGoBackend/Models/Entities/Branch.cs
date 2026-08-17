using InnGoHotel.Models;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InnGoBackend.Models
{
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BranchId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Address { get; set; }

        [Required, MaxLength(50)]
        public string City { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        public string? Description { get; set; }
        public string ImageUrl { get; set; }

        // Navigation Properties (1-N)
        [JsonIgnore]
        public ICollection<Room> Rooms { get; set; }
       
        [JsonIgnore]
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}