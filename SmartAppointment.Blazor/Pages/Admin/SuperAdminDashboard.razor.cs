using Microsoft.AspNetCore.Components;
using SmartAppointment.Blazor.Models;
using SmartAppointment.Blazor.Models.Admin;
using SmartAppointment.Blazor.Services;

namespace SmartAppointment.Blazor.Pages.Admin
{
    public class SuperAdminDashboardBase : ComponentBase
    {
        [Inject] protected AdminApiService AdminApi { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;

        protected List<AppointmentModel> Appointments { get; set; } = new();
        protected List<UserDto> Users { get; set; } = new();
        protected List<PermissionDto> Permissions { get; set; } = new();
        protected List<RoleDto> Roles { get; set; } = new();
        protected RoleDto? SelectedRole { get; set; }

        protected string? NewPermissionName { get; set; }
        protected string? NewPermissionDescription { get; set; }

        protected (int TotalUsers, int TotalRoles, int TotalPermissions) Stats { get; set; }
        protected (int TotalApproved, int TotalPending, int TotalRejected) AppointmentStats { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAllAsync();
        }

        protected async Task LoadAllAsync()
        {
            Permissions = (await AdminApi.GetPermissionsAsync()) ?? new();
            Roles = (await AdminApi.GetRolesWithPermissionsAsync()) ?? new();
            var users = await AdminApi.GetUsersWithRolesAsync() ?? new List<UserDto>();
            var allRoles = await AdminApi.GetRolesAsync() ?? new List<RoleDto>();
            Stats = (users.Count, allRoles.Count, Permissions.Count);
            var allAppointments = await AdminApi.GetAllAppointmentsAsync() ?? new List<AppointmentModel>();
            AppointmentStats = (
                TotalApproved: allAppointments.Count(a => a.Status == "Approved"),
                TotalPending: allAppointments.Count(a => a.Status == "Pending"),
                TotalRejected: allAppointments.Count(a => a.Status == "Rejected")
            );

            foreach (var p in Permissions) p.AssignedToSelectedRole = false;
        }

        protected void NavToPermissions() => Nav.NavigateTo("/admin/permissions");
        protected void NavToUsers() => Nav.NavigateTo("/admin/users");

        protected async Task CreatePermission()
        {
            if (string.IsNullOrWhiteSpace(NewPermissionName)) return;
            var dto = new { Name = NewPermissionName.Trim(), Description = NewPermissionDescription };
            var res = await AdminApi.CreatePermissionAsync(dto);
            if (res.IsSuccessStatusCode)
            {
                NewPermissionName = string.Empty;
                NewPermissionDescription = null;
                await LoadAllAsync();
            }
        }

        protected async Task DeletePermission(Guid id)
        {
            var res = await AdminApi.DeletePermissionAsync(id);
            if (res.IsSuccessStatusCode) await LoadAllAsync();
        }

        protected void OnRoleSelected(ChangeEventArgs e)
        {
            var roleId = e?.Value?.ToString();
            SelectedRole = Roles.FirstOrDefault(r => r.Id == roleId);
            foreach (var p in Permissions)
            {
                p.AssignedToSelectedRole = SelectedRole?.PermissionIds?.Contains(p.Id) ?? false;
            }
        }

        protected async Task SaveRolePermissions()
        {
            if (SelectedRole == null) return;
            var selectedIds = Permissions.Where(p => p.AssignedToSelectedRole).Select(p => p.Id).ToArray();
            var res = await AdminApi.SetRolePermissionsAsync(SelectedRole.Id, selectedIds);
            if (res.IsSuccessStatusCode)
            {
                SelectedRole.PermissionIds = selectedIds;
            }
        }
    }
}
