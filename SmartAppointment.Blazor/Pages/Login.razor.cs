using SmartAppointment.Blazor.Auth;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace SmartAppointment.Blazor.Pages
{
    public partial class Login
    {
        private LoginModel _model = new();
        private string? _error;

        private async Task HandleLogin()
        {
            var response = await Http.PostAsJsonAsync("https://localhost:7216/api/auth/login", _model);
            if (!response.IsSuccessStatusCode)
            {
                _error = "Invalid login attempt.";
                return;
            }
            var token = await response.Content.ReadAsStringAsync();

            await ((ApiAuthenticationStateProvider)AuthStateProvider).NotifyUserAuthenticationAsync(token);

            Navigation.NavigateTo("/appointments/create");
        }
        class LoginModel
        {
            [Required(ErrorMessage = "Please enter your Email or Username")]
            public string Identifier { get; set; } = string.Empty;
            public string Password { get; set; } = "";

        }
    }
}


