using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.Application.DTOs;
using SmartAppointment.Application.Interfaces;
using SmartAppointment.Infrastructure.Data;
using SmartAppointment.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SmartAppointment.API.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly INotificationService _notificationService;
        private readonly AppDbContext _db;

        public AppointmentsController(IAppointmentService appointmentService, UserManager<ApplicationUser> userManager, /*INotificationService notificationService*/ AppDbContext db)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
            //_notificationService = notificationService;
            _db = db;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();
                var userAppointments = await _appointmentService.GetByUserAsync(userId);
                return Ok(userAppointments);
            }
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            var callerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(callerId))
                return Unauthorized();

            //// if caller is admin, attempt to resolve provided CustomerName + SSN to an existing user.
            if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
            {
                if (string.IsNullOrWhiteSpace(dto.SSN))
                    return BadRequest("CustomerName and SSN are required when creating an appointment on behalf of a user.");

                var ssn = SanitizeSsn(dto.SSN);
                if (ssn.Length != 12)
                    return BadRequest("SSN must be exactly 12 digits.");

                //// find user by both username and SSN to avoid collisions
                var targetUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.SSN == ssn);

                if (targetUser == null)
                {
                    return BadRequest("Target user not found by provided CustomerName and SSN. Please register the user or activate the registration modal.");
                }

                dto.UserId = targetUser.Id;
            }
            else
            {
                dto.UserId = callerId;
            }

            if (!TryValidateModel(dto))
            {
                return BadRequest(ModelState);
            }

            await _appointmentService.CreateAsync(dto);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            try
            {
                await _appointmentService.ApproveAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        //for email notification by system when appointment is approved
        //activate this section and deactivate the one above
        //public async Task<IActionResult> Approve(Guid id)
        //{

        //    var appointment = await _db.Appointments.FindAsync(id);
        //    if (appointment == null) throw new Exception("Appointment not found");
        //    await _appointmentService.ApproveAsync(id);
        //    await _db.SaveChangesAsync();

        //    // Trigger the notifications
        //    await _notificationService.SendStatusUpdateAsync(
        //        appointment.CustomerName,
        //        appointment.Email,
        //        appointment.PhoneNumber,
        //        appointment.Date.ToString("dd/MM/yyyy HH:mm"),
        //        "Approved"
        //    );
        //}
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            try
            {
                await _appointmentService.RejectAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPatch("{id}/reschedule")]
        public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentDto dto)
        {
            if (dto == null)
                return BadRequest("Request body is required.");

            try
            {
                await _appointmentService.RescheduleAsync(id, dto.NewDate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}/cancelled")]
        public async Task<IActionResult> CancelAsync(Guid id)
        {
            try
            {
                await _appointmentService.CancelAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        private static string SanitizeSsn(string? ssn)
        {
            if (string.IsNullOrWhiteSpace(ssn)) return string.Empty;
            return Regex.Replace(ssn, @"\D", "");
        }
    }
}
