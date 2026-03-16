namespace SmartAppointment.Blazor.Models.Admin
{
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // UI flag only
        public bool AssignedToSelectedRole { get; set; }
    }
}
