namespace MaintenanceCenter.Application.DTOs.Technicians
{
    public class TechnicianDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int? WorkshopId { get; set; }
        public string? WorkshopName { get; set; } // Useful for the Admin grid view
    }

    public class CreateTechnicianDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; // e.g., tech_ahmed
        public string Password { get; set; } = string.Empty;
        public int WorkshopId { get; set; } // Must be assigned to a workshop
    }

    public class UpdateTechnicianDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        // Note: Password resets should be a separate endpoint/DTO for security, 
        // but we can allow changing the assigned workshop or name here.
        public int WorkshopId { get; set; }
    }
}