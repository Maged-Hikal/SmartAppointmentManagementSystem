using Blazored.LocalStorage;
using SmartAppointment.Blazor.Auth;

namespace SmartAppointment.Blazor.Services
{
    public class AuthService : IAuthService
    {
        private const string TokenKey = "authToken";
        private readonly ILocalStorageService _localStorage;

        public AuthService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetTokenAsync(string token)
        {
            await _localStorage.SetItemAsync(TokenKey, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _localStorage.GetItemAsync<string>(TokenKey);
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
        }
    }
}
