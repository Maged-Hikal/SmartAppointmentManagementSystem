namespace SmartAppointment.Blazor.Models.Admin
{
    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public Guid[]? PermissionIds { get; set; }
    }
}
