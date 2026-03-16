using SmartAppointment.Blazor.Auth;

namespace SmartAppointment.Blazor.Layout
{
    public partial class MainLayout
    {
        private async Task LogoutAsync()
        {
            if (AuthStateProvider is ApiAuthenticationStateProvider apiProvider)
            {
                apiProvider.NotifyUserLogout();
            }
            NavigationManager.NavigateTo("/login", forceLoad: false);
            await Task.CompletedTask;
        }
    }
}
