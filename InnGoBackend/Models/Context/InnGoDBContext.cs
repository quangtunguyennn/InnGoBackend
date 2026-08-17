using Microsoft.EntityFrameworkCore;
using InnGoBackend.Models;
using InnGoHotel.Models;
using InnGoBackend.Models.Entities;

namespace InnGoBackend.Models.Context
{
    public class InnGoDBContext : DbContext
    {
        public InnGoDBContext(DbContextOptions<InnGoDBContext> options) : base(options) { }

        public DbSet<User> User { get; set; } 
        public DbSet<Booking> Booking { get; set; }
        public DbSet<BookingService> BookingService { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Blog> Blog { get; set; }
    }
}
