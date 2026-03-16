namespace SmartAppointment.Domain.Entities
{
    public class RolePermission
    {
        // Composite key: RoleId + PermissionId
        public string RoleId { get; set; } = string.Empty;   // FK to AspNetRoles.Id
        public Guid PermissionId { get; set; }                // FK to Permissions.Id

        // Navigation properties
        // IdentityRole is part of Microsoft.AspNetCore.Identity; configured in DbContext
        public Permission? Permission { get; set; }
    }
}
