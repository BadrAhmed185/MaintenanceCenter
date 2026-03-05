using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "اسم الورشة مطلوب.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "اسم الورشة يجب أن يكون بين 3 و 100 حرف.")]
        // Allows letters, numbers, spaces, and dashes (e.g., "ورشة الإلكترونيات - 1")
        [RegularExpression(@"^[\p{L}\s\d\-]+$", ErrorMessage = "اسم الورشة يحتوي على رموز غير مسموحة.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف يجب ألا يتجاوز 500 حرف.")]
        public string? Description { get; set; }
    }

    public class UpdateWorkshopDto
    {
        [Required(ErrorMessage = "معرف الورشة مطلوب.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الورشة مطلوب.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "اسم الورشة يجب أن يكون بين 3 و 100 حرف.")]
        [RegularExpression(@"^[\p{L}\s\d\-]+$", ErrorMessage = "اسم الورشة يحتوي على رموز غير مسموحة.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف يجب ألا يتجاوز 500 حرف.")]
        public string? Description { get; set; }
    }
}