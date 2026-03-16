using Microsoft.JSInterop;
using SmartAppointment.Blazor.Models.Admin;

namespace SmartAppointment.Blazor.Pages.Admin
{
    public partial class Users
    {
        private bool IsModalOpen;
        private bool IsEditing;
        private EditUserModel EditModel = new();
        private string SearchTerm { get; set; } = "";
        private string FilterRole { get; set; } = "All";

        
        private List<UserDto> FilteredUsers
        {
            get
            {
                var query = Users?.AsEnumerable() ?? Enumerable.Empty<UserDto>();

                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(u =>
                        (u.UserName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.Email?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (u.SSN?.Contains(SearchTerm) ?? false)
                    );
                }

                if (FilterRole != "All")
                {
                    query = query.Where(u => u.Roles != null && u.Roles.Contains(FilterRole));
                }

                return query.ToList();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            Users = await AdminApi.GetUsersWithRolesAsync() ?? new List<UserDto>();
            StateHasChanged();
        }

        private void ShowCreate()
        {
            IsEditing = false;
            EditModel = new EditUserModel();
            IsModalOpen = true;
        }

        private void ShowEdit(UserDto user)
        {
            IsEditing = true;
            EditModel = new EditUserModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                SSN = user.SSN ?? string.Empty
            };
            IsModalOpen = true;
        }

        private void CloseModal()
        {
            IsModalOpen = false;
        }

        private async Task SaveUserAsync()
        {
            if (IsEditing)
            {
                var dto = new
                {
                    UserName = EditModel.UserName,
                    Email = EditModel.Email,
                    PhoneNumber = EditModel.PhoneNumber,
                    SSN = EditModel.SSN
                };

                var resp = await AdminApi.UpdateUserAsync(EditModel.Id!, dto);
                if (resp.IsSuccessStatusCode)
                {
                    await LoadUsersAsync();
                    CloseModal();
                }
                else
                {
                    var text = await resp.Content.ReadAsStringAsync();
                    await JS.InvokeVoidAsync("alert", $"Update failed: {text}");
                }
            }
            else
            {
                var dto = new
                {
                    UserName = EditModel.UserName,
                    Email = EditModel.Email,
                    PhoneNumber = EditModel.PhoneNumber,
                    SSN = EditModel.SSN,
                    Password = EditModel.Password,
                    Role = EditModel.Role
                };

                var resp = await AdminApi.CreateUserAsync(dto);
                if (resp.IsSuccessStatusCode)
                {
                    await LoadUsersAsync();
                    CloseModal();
                }
                else
                {
                    var text = await resp.Content.ReadAsStringAsync();
                    await JS.InvokeVoidAsync("alert", $"Create failed: {text}");
                }
            }
        }

        private async Task DeleteUserAsync(UserDto TargetUser)
        {
            if (await JS.InvokeAsync<bool>("confirm", $"Delete user {TargetUser.UserName}?"))
            {
                var resp = await AdminApi.DeleteUserAsync(TargetUser.Id);
                if (resp.IsSuccessStatusCode)
                {
                    await LoadUsersAsync();
                }
                else
                {
                    var text = await resp.Content.ReadAsStringAsync();
                    await JS.InvokeVoidAsync("alert", $"Delete failed: {text}");
                }
            }
        }

        private class EditUserModel
        {
            public string? Id { get; set; }
            public string UserName { get; set; } = "";
            public string Email { get; set; } = "";
            public string PhoneNumber { get; set; } = "";
            public string SSN { get; set; } = "";
            public string Password { get; set; } = "";
            public string Role { get; set; } = "User";
        }
    }
}
