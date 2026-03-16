using SmartAppointment.Blazor.Models;
using SmartAppointment.Blazor.Models.Admin;
using System.Net.Http.Json;

namespace SmartAppointment.Blazor.Services
{
    public class AdminApiService
    {
        private readonly HttpClient _http;

        public AdminApiService(HttpClient http)
        {
            _http = http;
        }
        public Task<List<AppointmentModel>?> GetAllAppointmentsAsync() =>
            _http.GetFromJsonAsync<List<AppointmentModel>>("api/appointments");

        public Task<List<PermissionDto>?> GetPermissionsAsync() =>
            _http.GetFromJsonAsync<List<PermissionDto>>("api/admin/permissions");

        public Task<List<RoleDto>?> GetRolesWithPermissionsAsync() =>
            _http.GetFromJsonAsync<List<RoleDto>>("api/admin/permissions/roles");

        public Task<HttpResponseMessage> CreatePermissionAsync(object dto) =>
            _http.PostAsJsonAsync("api/admin/permissions", dto);

        public Task<HttpResponseMessage> DeletePermissionAsync(Guid id) =>
            _http.DeleteAsync($"api/admin/permissions/{id}");

        public Task<HttpResponseMessage> SetRolePermissionsAsync(string roleId, Guid[] permissionIds) =>
            _http.PutAsJsonAsync($"api/admin/permissions/roles/{roleId}/permissions", permissionIds);

        public Task<List<UserDto>?> GetUsersWithRolesAsync() =>
            _http.GetFromJsonAsync<List<UserDto>>("api/admin/users");

        public Task<List<RoleDto>?> GetRolesAsync() =>
            _http.GetFromJsonAsync<List<RoleDto>>("api/admin/users/roles");

        public Task<HttpResponseMessage> SetUserRolesAsync(string userId, string[] roles) =>
            _http.PutAsJsonAsync($"api/admin/users/{userId}/roles", roles);

        // New endpoints for user CRUD
        public Task<HttpResponseMessage> CreateUserAsync(object dto) =>
            _http.PostAsJsonAsync("api/admin/users", dto);

        public Task<HttpResponseMessage> UpdateUserAsync(string userId, object dto) =>
            _http.PutAsJsonAsync($"api/admin/users/{userId}", dto);

        public Task<HttpResponseMessage> DeleteUserAsync(string userId) =>
            _http.DeleteAsync($"api/admin/users/{userId}");
    }
}
