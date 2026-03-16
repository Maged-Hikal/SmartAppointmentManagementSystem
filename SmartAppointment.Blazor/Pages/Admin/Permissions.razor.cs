using Microsoft.AspNetCore.Components;
using SmartAppointment.Blazor.Models.Admin;
using System.Net.Http.Json;

namespace SmartAppointment.Blazor.Pages.Admin
{
    public partial class Permissions
    {
        private List<PermissionDto> Permission { get; set; } = new();
        private List<RoleDto> Roles { get; set; } = new();
        private RoleDto? SelectedRole { get; set; }
        private string? NewPermissionName { get; set; }
        private string? NewPermissionDescription { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAll();
        }

        private async Task LoadAll()
        {
            Permission = (await Http.GetFromJsonAsync<List<PermissionDto>>("api/admin/permissions")) ?? new();
            Roles = (await Http.GetFromJsonAsync<List<RoleDto>>("api/admin/permissions/roles")) ?? new();

            foreach (var r in Roles)
            {
                r.PermissionIds ??= Array.Empty<Guid>();
            }

            foreach (var p in Permission)
            {
                p.AssignedToSelectedRole = false;
            }
        }

        private void Edit(Guid id)
        {
            NavManager.NavigateTo($"/admin/permissions/edit/{id}");
        }

        private async Task Delete(Guid id)
        {
            var res = await Http.DeleteAsync($"api/admin/permissions/{id}");
            if (res.IsSuccessStatusCode) await LoadAll();
        }

        private async Task CreatePermission()
        {
            if (string.IsNullOrWhiteSpace(NewPermissionName)) return;
            var dto = new { Name = NewPermissionName, Description = NewPermissionDescription };
            var res = await Http.PostAsJsonAsync("api/admin/permissions", dto);
            if (res.IsSuccessStatusCode)
            {
                NewPermissionName = string.Empty;
                NewPermissionDescription = null;
                await LoadAll();
            }
        }

        private void OnRoleChanged(ChangeEventArgs e)
        {
            var roleId = e?.Value?.ToString();
            SelectedRole = Roles.FirstOrDefault(r => r.Id == roleId);

            foreach (var p in Permission)
            {
                p.AssignedToSelectedRole = SelectedRole?.PermissionIds?.Contains(p.Id) ?? false;
            }
        }

        private async Task SaveRolePermissions()
        {
            if (SelectedRole == null) return;
            var selectedIds = Permission.Where(p => p.AssignedToSelectedRole).Select(p => p.Id).ToArray();
            var res = await Http.PutAsJsonAsync($"api/admin/permissions/roles/{SelectedRole.Id}/permissions", selectedIds);
            if (res.IsSuccessStatusCode)
            {
                SelectedRole.PermissionIds = selectedIds;
            }
        }
    }
}