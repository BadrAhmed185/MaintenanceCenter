using MaintenanceCenter.Domain.Common;
using MaintenanceCenter.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Entities
{
    // 6. Payment Receipt (إيصال الدفع المرفوع من الجهة)
    public class PaymentReceipt : AuditableEntity
    {
        public int MaintenanceRequestId { get; set; }
        public virtual MaintenanceRequest MaintenanceRequest { get; set; } = null!;

        public string ImageUrl { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? AdminNotes { get; set; } // In case of rejection (مرفوض لأن الصورة غير واضحة)
        public string? VerifiedById { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}
