using InnGoBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InnGoHotel.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        
        public string? MembershipTier { get; set; } = "Silver"; // Silver, Gold, Platinum
        
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties (1-N)
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
    
    }
}