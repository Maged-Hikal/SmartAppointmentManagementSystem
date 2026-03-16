using SmartAppointment.Blazor.Models;
using System.Net.Http.Json;
using static SmartAppointment.Blazor.Services.UserService;
using PasswordChangeRequest = SmartAppointment.Blazor.Models.PasswordChangeRequest;

namespace SmartAppointment.Blazor.Services
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetProfileAsync();
        Task<bool> UpdateProfileAsync(UserProfileDto? profile);
        Task<PasswordResult> ChangePasswordAsync(SmartAppointment.Blazor.Models.PasswordChangeRequest request);
    }
    public class UserService : IUserService
    {
        private readonly HttpClient _http;

        public UserService(HttpClient http)
        {
            _http = http;
        }

        public async Task<UserProfileDto?> GetProfileAsync()
        {
            try
            {
                // Notice: No {userId} needed here. The Token tells the server who you are.
                var response = await _http.GetAsync("api/me/profile");

                if (response.IsSuccessStatusCode)
                {
                    // If it's an Admin, the server returns 204 (NoContent), 
                    // so ReadFromJsonAsync might return null, which is perfect.
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        return null;

                    return await response.Content.ReadFromJsonAsync<UserProfileDto>();
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> UpdateProfileAsync(UserProfileDto profile)
        {
            try
            {
                // send the updated profile DTO to the server
                var response = await _http.PutAsJsonAsync("api/me/update-profile", profile);

                // Returns true if the server updated the record successfully (200 OK)
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<PasswordResult> ChangePasswordAsync(PasswordChangeRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/me/change-password", request);

                if (response.IsSuccessStatusCode)
                {
                    return new PasswordResult { IsSuccess = true };
                }

                // If it failed (e.g., wrong old password), we try to read the error message from the API
                var error = await response.Content.ReadFromJsonAsync<PasswordResult>();
                return error ?? new PasswordResult { IsSuccess = false, Message = "An unknown error occurred." };
            }
            catch (Exception ex)
            {
                return new PasswordResult { IsSuccess = false, Message = ex.Message };
            }
        }
        public class PasswordResult
        {
            public bool IsSuccess { get; set; }
            public string? Message { get; set; }
        }
    }
}