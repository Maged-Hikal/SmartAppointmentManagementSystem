namespace SmartAppointment.Blazor.Auth
{
    public class UserInfo
    {
        public bool IsAuthenticated { get; set; }
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();

    }

}
