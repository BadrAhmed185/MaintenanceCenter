namespace MaintenanceCenter.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // Optional: Helps you differentiate between Admin and Receptionist later
        public string Role { get; set; } = "Receptionist";
    }
}