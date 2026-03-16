using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.Blazor.Models;
using SmartAppointment.Infrastructure.Data;
using SmartAppointment.Infrastructure.Identity;
using System.Security.Claims;

namespace SmartAppointment.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/me")]
    [EnableRateLimiting("api")]
    public class UserPermissionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserPermissionsController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> GetMyPermissions()
        {
            var userId = User?.Claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any()) return Ok(new string[0]);

            var roleIds = await _db.Set<IdentityRole>()
                                   .Where(r => roles.Contains(r.Name ?? string.Empty))
                                   .Select(r => r.Id)
                                   .ToListAsync();

            var perms = await _db.RolePermissions
                                .Include(rp => rp.Permission)
                                .Where(rp => roleIds.Contains(rp.RoleId) && rp.Permission != null)
                                .Select(rp => rp.Permission!.Name)
                                .Distinct()
                                .ToListAsync();

            return Ok(perms);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                return NoContent(); // Or return a blank DTO
            }

            var userProfile = await _db.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => new UserProfileDto
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    SSN = u.SSN
                })
                .FirstOrDefaultAsync();

            if (userProfile == null) return NotFound("User profile not found.");

            return Ok(userProfile);
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(currentUserId ?? string.Empty);

            if (user == null) return NotFound();

            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return Ok();

            return BadRequest(result.Errors);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest model)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(currentUserId ?? string.Empty);

            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { IsSuccess = true });
            }

            return BadRequest(new { IsSuccess = false, Message = result.Errors.FirstOrDefault()?.Description });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            bool ssnExists = await _userManager.Users.AnyAsync(u => u.SSN == model.SSN);

            if (ssnExists)
            {
                return BadRequest(new[] { "SSN already registered, please choose another SSN" });
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                SSN = model.SSN
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok();
            }

            var errorList = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(errorList);
        }
    }
}
