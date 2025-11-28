//Sistema-Eventos\Models\Category.cs
using System.ComponentModel.DataAnnotations;

namespace Sistema_Eventos.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // Único

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}