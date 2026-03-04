using MaintenanceCenter.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceCenter.Application.DTOs.MaintenanceRequests
{
    public class MaintenanceRequestDto
    {
        public int Id { get; set; }
        public string TrackingCode { get; set; } = string.Empty;

        public string DeviceName { get; set; } = string.Empty;
        public string FaultDescription { get; set; } = string.Empty;
        public string ClientEntityName { get; set; } = string.Empty;
        public string DelivererName { get; set; } = string.Empty;
        public string DelivererPhone { get; set; } = string.Empty;
        public string DeviceCondition { get; set; } = string.Empty;

        public DeviceStatus Status { get; set; }
        public string StatusName => Status.ToString(); // Or map to Arabic later in UI

        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }

        // Associated Users/Entities
        public string? ReceptionistName { get; set; }
        public string? WorkshopName { get; set; }
        public string? TechnicianName { get; set; }
    }

        public class CreateMaintenanceRequestDto
        {
            [Required(ErrorMessage = "اسم الجهاز مطلوب.")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "اسم الجهاز يجب أن يكون بين 3 و 100 حرف.")]
            public string DeviceName { get; set; } = string.Empty;

            [Required(ErrorMessage = "وصف العطل مطلوب.")]
            [StringLength(500, MinimumLength = 4, ErrorMessage = "وصف العطل يجب أن يكون 4 أحرف على الأقل.")]
            public string FaultDescription { get; set; } = string.Empty;

            [Required(ErrorMessage = "الجهة القادم منها الجهاز مطلوبة.")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "اسم الجهة يجب أن يكون 3 أحرف على الأقل.")]
            public string ClientEntityName { get; set; } = string.Empty;

            [Required(ErrorMessage = "اسم الشخص المُسلّم مطلوب.")]
            // \p{L} matches any kind of letter from any language (including Arabic), \s matches spaces.
            [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "اسم المُسلّم يجب أن يحتوي على حروف ومسافات فقط بدون أرقام أو رموز.")]
            [StringLength(100, MinimumLength = 3, ErrorMessage = "اسم المُسلّم يجب أن يكون 3 أحرف على الأقل.")]
            public string DelivererName { get; set; } = string.Empty;

            [Required(ErrorMessage = "رقم هاتف المُسلّم مطلوب.")]
            // Strict Egyptian mobile number format: Starts with 01, then 0,1,2 or 5, followed by 8 digits.
            [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "رقم الهاتف غير صحيح، يجب أن يكون رقم موبايل مصري صحيح (مثال: 01012345678).")]
            public string DelivererPhone { get; set; } = string.Empty;

            [MaxLength(500, ErrorMessage = "وصف حالة الجهاز يجب ألا يتجاوز 500 حرف.")]
            public string DeviceCondition { get; set; } = string.Empty;
        }
    }



// It is done and the authentication problem is solved, but we have a major disaster here , you are supposed to be my architect but i found my self your  architect , please please focus and think like my architect to not fall in like these mistakes again.

//The problem is that he have no any validations either client-side or server-side , when i sumbitted a valid data t