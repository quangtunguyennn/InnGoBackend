using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace InnGoBackend.Models.Entities
{
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Summary { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ThumbnailUrl { get; set; }

  

        [MaxLength(20)]
        public string Status { get; set; } = "Published";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
