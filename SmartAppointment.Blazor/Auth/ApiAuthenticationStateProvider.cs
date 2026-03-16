using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace SmartAppointment.Blazor.Auth
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;
        public ApiAuthenticationStateProvider(IAuthService authService)
        {
            _authService = authService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            var claims = JwtParser.ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task NotifyUserAuthenticationAsync(string rawToken)
        {
            if (string.IsNullOrWhiteSpace(rawToken))
            {
                return;
            }

            var token = NormalizeToken(rawToken);
            await _authService.SetTokenAsync(token);

            var claims = JwtParser.ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)))
                );
        }


        public void NotifyUserLogout()
        {
            _authService.LogoutAsync().ContinueWith(_ =>
            {
                NotifyAuthenticationStateChanged(
                    Task.FromResult(
                        new AuthenticationState(
                            new ClaimsPrincipal(new ClaimsIdentity())
                        )
                    )
                );
            });
        }
        private static string NormalizeToken(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return raw!;
            try
            {
                var maybeString = JsonSerializer.Deserialize<string>(raw);
                if (!string.IsNullOrEmpty(maybeString))
                    return maybeString;
            }
            catch
            {
                // ignore; try next
            }

            try
            {
                using var doc = JsonDocument.Parse(raw);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    if (doc.RootElement.TryGetProperty("token", out var t) && t.ValueKind == JsonValueKind.String)
                        return t.GetString() ?? raw;

                    if (doc.RootElement.TryGetProperty("accessToken", out var at) && at.ValueKind == JsonValueKind.String)
                        return at.GetString() ?? raw;

                    if (doc.RootElement.TryGetProperty("access_token", out var aco) && aco.ValueKind == JsonValueKind.String)
                        return aco.GetString() ?? raw;
                }
            }
            catch
            {
                // ignore; fallback
            }

            if (raw.Length >= 2 && raw[0] == '"' && raw[^1] == '"')
            {
                return raw.Substring(1, raw.Length - 2);
            }

            // fallback: assume raw is already the token
            return raw;
        }
    }
    public interface IAuthService
    {
        Task SetTokenAsync(string token);
        Task<string?> GetTokenAsync();
        Task LogoutAsync();
    }
}
