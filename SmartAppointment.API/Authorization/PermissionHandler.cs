using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.Infrastructure.Data;
using SmartAppointment.Infrastructure.Identity;
using System.Security.Claims;

namespace SmartAppointment.API.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionHandler(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any()) return;

            var roleIds = await _db.Set<IdentityRole>()
                                   .Where(r => roles.Contains(r.Name))
                                   .Select(r => r.Id)
                                   .ToListAsync();

            var has = await _db.RolePermissions
                               .Include(rp => rp.Permission)
                               .Where(rp => roleIds.Contains(rp.RoleId) && rp.Permission!.Name == requirement.Permission)
                               .AnyAsync();

            if (has)
            {
                context.Succeed(requirement);
            }
        }
    }
}
