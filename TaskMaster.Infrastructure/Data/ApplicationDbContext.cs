// TaskMaster.Infrastructure/Data/ApplicationDbContext.cs (modificaciones)

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid> // Hereda de IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet<User> ya no es necesario, lo gestiona IdentityDbContext
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Renombrar tablas de Identity para evitar conflictos o por preferencia
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            // Configuración de relaciones y propiedades (ajustadas para IdentityUser<Guid>)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar proyectos si se elimina el dueño

            modelBuilder.Entity<User>()
                .HasMany(u => u.AssignedTasks)
                .WithOne(t => t.AssignedTo)
                .HasForeignKey(t => t.AssignedToId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull); // Asignar a null si el usuario asignado es eliminado

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar tareas si se elimina el proyecto

            // Asegurar que las propiedades de cadena no sean nulas por defecto
            modelBuilder.Entity<Project>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<TaskItem>().Property(t => t.Title).IsRequired();
        }

        // Añadir este método para aplicar migraciones si se usa una DB real
        public void Migrate()
        {
            Database.Migrate();
        }
    }
}