using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.Domain.Entities;
using SmartAppointment.Infrastructure.Data;

namespace SmartAppointment.API.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionsController(AppDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _db.Permissions
                .Select(p => new { p.Id, p.Name, p.Description })
                .ToListAsync();
            return Ok(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PermissionCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name required.");
            if (await _db.Permissions.AnyAsync(p => p.Name == dto.Name))
                return Conflict("Permission already exists.");

            var permission = new Permission { Id = Guid.NewGuid(), Name = dto.Name.Trim(), Description = dto.Description };
            _db.Permissions.Add(permission);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = permission.Id }, permission);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PermissionUpdateDto dto)
        {
            var perm = await _db.Permissions.FindAsync(id);
            if (perm == null) return NotFound();
            if (!string.IsNullOrWhiteSpace(dto.Name)) perm.Name = dto.Name.Trim();
            perm.Description = dto.Description;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var perm = await _db.Permissions.FindAsync(id);
            if (perm == null) return NotFound();

            var rolePerms = _db.RolePermissions.Where(rp => rp.PermissionId == id);
            _db.RolePermissions.RemoveRange(rolePerms);

            _db.Permissions.Remove(perm);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRolesWithPermissions()
        {
            var roles = await _roleManager.Roles
                .Select(r => new { r.Id, r.Name })
                .ToListAsync();

            var rolePermissions = await _db.RolePermissions
                .Select(rp => new { rp.RoleId, rp.PermissionId })
                .ToListAsync();

            var result = roles.Select(r => new
            {
                r.Id,
                r.Name,
                PermissionIds = rolePermissions.Where(x => x.RoleId == r.Id).Select(x => x.PermissionId).ToArray()
            });

            return Ok(result);
        }

        [HttpPut("roles/{roleId}/permissions")]         // Replace role permissions in one call (idempotent)
        public async Task<IActionResult> SetRolePermissions(string roleId, [FromBody] Guid[] permissionIds)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound("Role not found.");

            var existing = _db.RolePermissions.Where(rp => rp.RoleId == roleId);
            _db.RolePermissions.RemoveRange(existing);

            var validPermissionIds = await _db.Permissions.Where(p => permissionIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();
            foreach (var pid in validPermissionIds)
            {
                _db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = pid });
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    public record PermissionCreateDto(string Name, string? Description);
    public record PermissionUpdateDto(string? Name, string? Description);
}
