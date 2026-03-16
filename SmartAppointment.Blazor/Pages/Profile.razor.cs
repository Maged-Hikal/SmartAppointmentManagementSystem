using Microsoft.AspNetCore.Components;
using SmartAppointment.Blazor.Models;
using SmartAppointment.Blazor.Services;

namespace SmartAppointment.Blazor.Pages
{
    public partial class Profile
    {
        [Inject] protected IUserService UserService { get; set; } = default!;

        private UserProfileDto userModel;
        private PasswordChangeRequest passwordModel = new();



        private bool profileSaved = false;
        private bool passSuccess = false;
        private string passError = "";

        private bool showPassword = false;

        private void TogglePasswordVisibility()
        {
            showPassword = !showPassword;
        }
        protected override async Task OnInitializedAsync()
        {
            userModel = await UserService.GetProfileAsync();
        }

        private async Task UpdateProfile()
        {
            profileSaved = false;
            var success = await UserService.UpdateProfileAsync(userModel);
            if (success)
            {
                profileSaved = true;
                StateHasChanged();
                await Task.Delay(3000);
                profileSaved = false;
            }
        }

        private async Task ChangePassword()
        {
            passError = "";
            passSuccess = false;



            var result = await UserService.ChangePasswordAsync(passwordModel);
            if (result.IsSuccess)
            {
                passSuccess = true;
                passwordModel = new(); // Reset form
                StateHasChanged();
            }
            else
            {
                passError = result.Message;
            }
        }
    }
}
