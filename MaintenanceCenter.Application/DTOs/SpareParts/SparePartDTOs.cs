using System.ComponentModel.DataAnnotations;

namespace MaintenanceCenter.Application.DTOs.SpareParts
{
    public class SparePartDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }

    public class CreateSparePartDto
    {
        [Required(ErrorMessage = "اسم قطعة الغيار مطلوب.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم قطعة الغيار يجب أن يكون بين 2 و 100 حرف.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "تكلفة القطعة مطلوبة.")]
        [Range(0.01, 1000000, ErrorMessage = "التكلفة يجب أن تكون مبلغاً صحيحاً أكبر من صفر.")]
        public decimal CurrentCost { get; set; }
    }

    public class UpdateSparePartDto
    {
        [Required(ErrorMessage = "معرف القطعة مطلوب.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم قطعة الغيار مطلوب.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "اسم قطعة الغيار يجب أن يكون بين 2 و 100 حرف.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "تكلفة القطعة مطلوبة.")]
        [Range(0.01, 1000000, ErrorMessage = "التكلفة يجب أن تكون مبلغاً صحيحاً أكبر من صفر.")]
        public decimal CurrentCost { get; set; }
    }
}