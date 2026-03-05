using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Application.DTOs.MaintenanceServices
{
    public class MaintenanceServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }

    public class CreateMaintenanceServiceDto
    {

        [Required(ErrorMessage = "اسم الخدمة مطلوب.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم الخدمة يجب أن يكون بين 2 و 100 حرف.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "تكلفة الخدمة مطلوبة.")]
        [Range(0.01, 1000000, ErrorMessage = "التكلفة يجب أن تكون مبلغاً صحيحاً أكبر من صفر.")]
        public decimal CurrentCost { get; set; }
    }

    public class UpdateMaintenanceServiceDto
    {
        [Required(ErrorMessage = "معرف الخدمة مطلوب.")]

        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الخدمة مطلوب.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم الخدمة يجب أن يكون بين 2 و 100 حرف.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "تكلفة الخدمة مطلوبة.")]
        [Range(0.01, 1000000, ErrorMessage = "التكلفة يجب أن تكون مبلغاً صحيحاً أكبر من صفر.")]
        public decimal CurrentCost { get; set; }
    }
}
