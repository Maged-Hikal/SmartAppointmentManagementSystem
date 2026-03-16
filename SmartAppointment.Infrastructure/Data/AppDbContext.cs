using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.Domain.Entities;
using SmartAppointment.Infrastructure.Identity;

namespace SmartAppointment.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Appointment>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.UserId)
                 .HasColumnType("nvarchar(450)")
                 .IsRequired();
                b.HasOne<ApplicationUser>()
                 .WithMany()
                 .HasForeignKey(a => a.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Permission
            modelbuilder.Entity<Permission>(p =>
            {
                p.HasKey(x => x.Id);
                p.Property(x => x.Name)
                 .IsRequired()
                 .HasMaxLength(200)
                 .HasColumnType("nvarchar(200)");
            });

            // RolePermission join
            modelbuilder.Entity<RolePermission>(rp =>
            {
                rp.HasKey(x => new { x.RoleId, x.PermissionId });

                rp.HasOne(r => r.Permission)
                  .WithMany(p => p.RolePermissions)
                  .HasForeignKey(r => r.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);

                // link to AspNetRoles table (IdentityRole)
                rp.HasOne<IdentityRole>()
                  .WithMany()
                  .HasForeignKey(r => r.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
