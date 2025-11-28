using Microsoft.EntityFrameworkCore;
using Sistema_Eventos.Models; // Importante: Referencia a tu carpeta de entidades

namespace Sistema_Eventos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Representación de las tablas en la BD
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales (Fluent API)

            // Asegurar que el email sea único
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Asegurar que el nombre de categoría sea único
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Configuración de precisión para decimales (Precios y Coordenadas)
            // Aunque usamos atributos [Column] en los modelos, esto es un refuerzo.
            modelBuilder.Entity<Event>()
                .Property(e => e.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Event>()
                .Property(e => e.Latitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Event>()
                .Property(e => e.Longitude)
                .HasPrecision(9, 6);
        }
    }
}