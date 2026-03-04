namespace MaintenanceCenter.Application.DTOs.Workshops
{
    // 1. For returning data to the client
    public class WorkshopDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TechniciansCount { get; set; } // Useful for the Admin dashboard
    }

    // 2. For creating a new workshop
    public class CreateWorkshopDto
    {
        // Data Annotations can be added here or handled via FluentValidation later
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    // 3. For updating an existing workshop
    public class UpdateWorkshopDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}