using Blazored.LocalStorage;
using SmartAppointment.Blazor.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
namespace SmartAppointment.Blazor.Services
{
    public class AppointmentApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localstorage;

        public AppointmentApiService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _localstorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        }

        public async Task<List<AppointmentModel>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AppointmentModel>>("api/appointments") ?? new();
        }

        public async Task CreateAsync(CreateAppointmentModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var token = await _localstorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrEmpty(token))
                throw new Exception("User is not authenticated. Token is missing.");

            token = NormalizeToken(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize using PascalCase property names so server receives "CustomerName" exactly
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null // preserve PascalCase
            };
            var json = JsonSerializer.Serialize(model, options);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/appointments", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create appointment: {(int)response.StatusCode} {response.ReasonPhrase} {error}");
            }
        }

        public async Task ApproveAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/appointments/{id}/approve", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to approve appointment: {(int)response.StatusCode} {response.ReasonPhrase} {error}");
            }
        }
        public async Task RejectAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/appointments/{id}/reject");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to reject appointment: {(int)response.StatusCode} {response.ReasonPhrase} {error}");
            }
        }
        public async Task CancelAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/appointments/{id}/cancelled", null);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to cancel appointment : {(int)response.StatusCode} {response.ReasonPhrase} {error}");
            }
        }
        public async Task RescheduleAsync(Guid id, DateTime newDate)
        {
            var finalDateTime = newDate;

            var payload = new { NewDate = finalDateTime };
            var options = new JsonSerializerOptions { PropertyNamingPolicy = null }; // preserve PascalCase
            var json = JsonSerializer.Serialize(payload, options);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"api/appointments/{id}/reschedule", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to reschedule appointment: {(int)response.StatusCode} {response.ReasonPhrase} {error}");
            }
        }

        private static string NormalizeToken(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return raw ?? string.Empty;

            try
            {
                var maybeString = JsonSerializer.Deserialize<string>(raw);
                if (!string.IsNullOrEmpty(maybeString))
                    return maybeString;
            }
            catch { }

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
            catch { }

            if (raw.Length >= 2 && raw[0] == '"' && raw[^1] == '"')
            {
                return raw.Substring(1, raw.Length - 2);
            }

            return raw;
        }
    }
}

