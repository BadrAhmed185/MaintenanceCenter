using MaintenanceCenter.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceCenter.Application.DTOs.MaintenanceRequests
{
    public class SelectedSparePartDto
    {
        [Required]
        public int SparePartId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "الكمية يجب أن تكون 1 على الأقل.")]
        public int Quantity { get; set; }
    }

    public class SubmitInspectionDto
    {
        [Required]
        public int RequestId { get; set; }

        public string TechnicalReport { get; set; } = string.Empty;

        public bool IsRepairable { get; set; } = true;

        // Technician can manually dictate the next state (e.g., QuotationReady, UnderRepair, Delivered)
        public DeviceStatus Status { get; set; }

        // The manual financial override
        [Range(0, 1000000, ErrorMessage = "التكلفة الإجمالية غير صالحة.")]
        public decimal TotalCost { get; set; }

        public List<SelectedSparePartDto> SelectedParts { get; set; } = new();
        public List<int> SelectedServices { get; set; } = new();
    }
}