using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SmartAppointment.Blazor.Models;
using SmartAppointment.Blazor.Services;

namespace SmartAppointment.Blazor.Pages
{
    public partial class CreateAppointment
    {
        private CreateAppointmentModel model = new();
        private string? _error;
        private bool _isSuccess;
        private bool _isSubmitting;
        private bool IsReadOnly = false;

        [Inject] protected NavigationManager Nav { get; set; } = default!;
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] protected IUserService UserService { get; set; } = default!; // Your service to fetch user data


        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is { IsAuthenticated: true })
            {
                bool isAdmin = user.IsInRole("Admin");
                bool isSuperAdmin = user.IsInRole("SuperAdmin");
                if (!isAdmin && !isSuperAdmin)
                {
                    IsReadOnly = true; // LOCK the form fields

                    var profile = await UserService.GetProfileAsync();
                    if (profile != null)
                    {
                        model.CustomerName = profile.UserName;
                        model.SSN = profile.SSN;
                        model.Email = profile.Email;
                        model.PhoneNumber = profile.PhoneNumber;
                    }
                }
                else
                {
                    IsReadOnly = false; // UNLOCK for Admins
                }
            }
        }
        // registration modal state for inline flow
        private bool showRegistrationModal = false;

        private async Task HandleSubmit()
        {
            _error = null;
            _isSuccess = false;
            _isSubmitting = true;

            try
            {
                await Api.CreateAsync(model);

                _isSuccess = true;
                StateHasChanged();

                await Task.Delay(2000);

                Nav.NavigateTo("/appointments");
            }
            catch (Exception ex)
            {
                var msg = ex.Message ?? string.Empty;
                if (msg.Contains("Target user not found", StringComparison.OrdinalIgnoreCase))
                {
                    showRegistrationModal = true;
                }
                else
                {
                    _error = "Something went wrong: " + msg;
                }
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private async Task OnUserRegistered()
        {
            try
            {
                _isSubmitting = true;
                await Api.CreateAsync(model);
                model = new();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
            finally
            {
                _isSubmitting = false;
            }
        }
    }
}
