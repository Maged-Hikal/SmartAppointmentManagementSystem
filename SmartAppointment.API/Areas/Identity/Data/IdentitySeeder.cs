using Microsoft.AspNetCore.Identity;
using SmartAppointment.Domain.Entities;
using SmartAppointment.Infrastructure.Data;
using SmartAppointment.Infrastructure.Identity;

namespace SmartAppointment.API.Areas.Identity.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var db = services.GetRequiredService<AppDbContext>();

            // Roles
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Permissions - add as needed
            var permissionsToEnsure = new[]
            {
                ("Appointments.View", "View appointments"),
                ("Appointments.Create", "Create appointments"),
                ("Appointments.Reschedule", "Reschedule appointments"),
                ("Appointments.Cancel", "Cancel appointments"),
                ("Appointments.Approve", "Approve appointments"),
                ("Appointments.Reject", "Reject appointments")
            };

            foreach (var (name, desc) in permissionsToEnsure)
            {
                var exists = db.Permissions.Any(p => p.Name == name);
                if (!exists)
                {
                    db.Permissions.Add(new Permission { Id = Guid.NewGuid(), Name = name, Description = desc });
                }
            }

            await db.SaveChangesAsync();

            // Map permissions to roles: SuperAdmin -> all, Admin -> many, User -> view/create maybe
            async Task EnsureRoleHasPermission(string roleName, string permissionName)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null) return;

                var permission = db.Permissions.SingleOrDefault(p => p.Name == permissionName);
                if (permission == null) return;

                var exists = db.RolePermissions.Any(rp => rp.RoleId == role.Id && rp.PermissionId == permission.Id);
                if (!exists)
                {
                    db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
                }
            }

            // Give everything to SuperAdmin
            var allPermissions = db.Permissions.Select(p => p.Name).ToList();
            foreach (var perm in allPermissions)
            {
                await EnsureRoleHasPermission("SuperAdmin", perm);
            }

            // Admin subset
            foreach (var perm in new[] { "Appointments.View", "Appointments.Create", "Appointments.Reschedule", "Appointments.Cancel", "Appoinments.Delete", "Appointments.Approve" })
            {
                await EnsureRoleHasPermission("Admin", perm);
            }

            // User permissions
            foreach (var perm in new[] { "Appointments.View", "Appointments.Create", "Appointments.Reschedule", "Appointments.Cancel" })
            {
                await EnsureRoleHasPermission("User", perm);
            }
            await db.SaveChangesAsync();

            // Create users as before (unchanged)
            var superAdminEmail = "superadmin@smartapp.com";
            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdminUser == null)
            {
                superAdminUser = new ApplicationUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(superAdminUser, "SuperAdmin123!");
                await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
            }
            var adminEmail = "admin@smartapp.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            var userEmail = "user@smartapp.com";
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "User123!");
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
