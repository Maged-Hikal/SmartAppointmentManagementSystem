using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SmartAppointment.Blazor.Pages
{
    public partial class UserRegistrationModal
    {
        [Parameter] public bool Visible { get; set; }
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        [Parameter] public bool IsEditing { get; set; } = false;
        [Parameter] public string? UserId { get; set; }

        [Parameter] public string? InitialUserName { get; set; }
        [Parameter] public string? InitialEmail { get; set; }
        [Parameter] public string? InitialPhoneNumber { get; set; }
        [Parameter] public string? InitialSSN { get; set; }

        [Parameter] public EventCallback OnSaved { get; set; }

        private CreateUserRequestModel Model = new();

        protected override void OnParametersSet()
        {
            // Prefill when parameters arrive
            Model.UserName = InitialUserName ?? string.Empty;
            Model.Email = InitialEmail ?? string.Empty;
            Model.PhoneNumber = InitialPhoneNumber ?? string.Empty;
            Model.SSN = InitialSSN ?? string.Empty;

            if (IsEditing && string.IsNullOrEmpty(Model.Password))
            {
                Model.Password = string.Empty;
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                var sanitizedUserName = GenerateConformingUserName(Model.UserName, Model.Email);

                if (IsEditing && !string.IsNullOrEmpty(UserId))
                {
                    var dto = new { UserName = sanitizedUserName, Email = Model.Email, PhoneNumber = Model.PhoneNumber, SSN = Model.SSN };
                    var resp = await AdminApi.UpdateUserAsync(UserId, dto);
                    if (resp.IsSuccessStatusCode)
                    {
                        await NotifySaved();
                    }
                    else
                    {
                        var txt = await resp.Content.ReadAsStringAsync();
                        await JS.InvokeVoidAsync("alert", $"Update failed: {txt}");
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Model.Password))
                    {
                        await JS.InvokeVoidAsync("alert", "Password is required for creating a user.");
                        return;
                    }

                    var dto = new
                    {
                        UserName = sanitizedUserName,
                        Email = Model.Email,
                        PhoneNumber = Model.PhoneNumber,
                        SSN = Model.SSN,
                        Password = Model.Password,
                    };
                    var resp = await AdminApi.CreateUserAsync(dto);
                    if (resp.IsSuccessStatusCode)
                    {
                        await NotifySaved();
                    }
                    else
                    {
                        var txt = await resp.Content.ReadAsStringAsync();
                        await JS.InvokeVoidAsync("alert", $"Create failed: {txt}");
                    }
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
        }

        private static string GenerateConformingUserName(string? inputName, string? email)
        {
            string candidate = (inputName ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(candidate) && !string.IsNullOrEmpty(email))
            {
                var at = email.IndexOf('@');
                candidate = at > 0 ? email.Substring(0, at) : email;
            }

            // Remove all characters other than letters and digits (Identity's default Username validator).
            candidate = Regex.Replace(candidate, @"[^a-zA-Z0-9]", string.Empty);

            if (string.IsNullOrEmpty(candidate))
            {
                // fallback stable username
                candidate = "user" + Guid.NewGuid().ToString("N").Substring(0, 6);
            }

            return candidate;
        }

        private async Task NotifySaved()
        {
            await SetVisible(false);
            if (OnSaved.HasDelegate) await OnSaved.InvokeAsync();
        }

        private async Task Cancel()
        {
            await SetVisible(false);
        }

        private async Task SetVisible(bool v)
        {
            Visible = v;
            await VisibleChanged.InvokeAsync(v);
        }

        private class CreateUserRequestModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "Name is too long.")]
            [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Only letters and name symbols (- ') are allowed.")]
            public string? UserName { get; set; }

            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            [Required]
            [RegularExpression(@"^\d{12}$", ErrorMessage = "SSN must be exactly 12 digits.")]
            [StringLength(12, MinimumLength = 12)]
            public string? SSN { get; set; }
            public string? Password { get; set; }
        }
    }
}
