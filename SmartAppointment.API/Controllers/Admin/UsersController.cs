using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.Infrastructure.Identity;
using System.Text.RegularExpressions;

namespace SmartAppointment.API.Controllers.Admin
{
    [Authorize(Roles = "SuperAdmin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.SSN
                })
                .ToListAsync();

            var result = new System.Collections.Generic.List<object>();
            foreach (var u in users)
            {
                var user = await _userManager.FindByIdAsync(u.Id);
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new { u.Id, u.UserName, u.Email, u.PhoneNumber, u.SSN, Roles = roles });
            }

            return Ok(result);
        }

        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
            return Ok(roles);
        }

        [HttpPut("{userId}/roles")]
        public async Task<IActionResult> SetUserRoles(string userId, [FromBody] string[] roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var current = await _userManager.GetRolesAsync(user);

            var toRemove = current.Except(roles).ToArray();
            var toAdd = roles.Except(current).ToArray();

            if (toRemove.Any())
            {
                var remResult = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!remResult.Succeeded) return StatusCode(500, remResult.Errors);
            }

            if (toAdd.Any())
            {
                var invalidRoles = toAdd.Where(r => !_roleManager.RoleExistsAsync(r).Result).ToList();
                if (invalidRoles.Any()) return BadRequest(new { message = "Invalid roles: " + string.Join(",", invalidRoles) });

                var addResult = await _userManager.AddToRolesAsync(user, toAdd);
                if (!addResult.Succeeded) return StatusCode(500, addResult.Errors);
            }

            return NoContent();
        }

        public class CreateUserRequest
        {
            public string? UserName { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? SSN { get; set; }
            public string? Password { get; set; }
            public string? Role { get; set; }
        }

        public class UpdateUserRequest
        {
            public string? UserName { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? SSN { get; set; }

        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("UserName and Password are required.");

            var ssn = SanitizeSsn(dto.SSN);
            if (ssn.Length != 12) return BadRequest("SSN must be 12 digits.");

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                SSN = ssn
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            await _userManager.AddToRoleAsync(user, dto.Role);
            await _userManager.AddToRoleAsync(user, "User");

            return CreatedAtAction(nameof(GetUsersWithRoles), new { id = user.Id }, new { user.Id, user.UserName, user.Email, user.PhoneNumber, user.SSN });
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequest dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            if (!string.IsNullOrWhiteSpace(dto.UserName)) user.UserName = dto.UserName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.SSN))
            {
                var ssn = SanitizeSsn(dto.SSN);
                if (ssn.Length != 12) return BadRequest("SSN must be 12 digits.");
                user.SSN = ssn;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) return StatusCode(500, updateResult.Errors);

            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return StatusCode(500, result.Errors);

            return NoContent();
        }

        private static string SanitizeSsn(string? ssn)
        {
            if (string.IsNullOrWhiteSpace(ssn)) return string.Empty;
            return Regex.Replace(ssn, @"\D", "");
        }
    }
}
