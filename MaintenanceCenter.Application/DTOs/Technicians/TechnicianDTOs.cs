using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "الاسم بالكامل مطلوب.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "الاسم يجب أن يكون بين 3 و 100 حرف.")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "الاسم يجب أن يحتوي على حروف ومسافات فقط.")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "اسم المستخدم يجب أن يكون بين 4 و 50 حرف.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "اسم المستخدم يجب أن يحتوي على حروف إنجليزية، أرقام، وشرطة سفلية فقط.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "يجب تحديد الورشة التي يتبع لها الفني.")]
        [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار ورشة صحيحة.")]
        public int WorkshopId { get; set; }
    }

    public class UpdateTechnicianDto
    {
        [Required(ErrorMessage = "معرف الفني مطلوب.")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم بالكامل مطلوب.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "الاسم يجب أن يكون بين 3 و 100 حرف.")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "الاسم يجب أن يحتوي على حروف ومسافات فقط.")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "يجب تحديد الورشة التي يتبع لها الفني.")]
        [Range(1, int.MaxValue, ErrorMessage = "يرجى اختيار ورشة صحيحة.")]
        public int WorkshopId { get; set; }
    }
}