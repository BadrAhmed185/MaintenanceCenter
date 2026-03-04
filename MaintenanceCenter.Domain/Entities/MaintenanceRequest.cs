using MaintenanceCenter.Domain.Common;
using MaintenanceCenter.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Entities
{
    // 3. The Core State Machine: Maintenance Request (طلب الصيانة / الجهاز)
    public class MaintenanceRequest : AuditableEntity
    {
        public string TrackingCode { get; set; } = string.Empty; // e.g., REQ-2026-A4B9

        // Device Data (Receptionist Entry)
        public string DeviceName { get; set; } = string.Empty;
        public string FaultDescription { get; set; } = string.Empty;
        public string ClientEntityName { get; set; } = string.Empty; // الجهة القادم منها
        public string DelivererName { get; set; } = string.Empty;    // اسم المسلم
        public string DelivererPhone { get; set; } = string.Empty;   // تليفون المسلم
        public string DeviceCondition { get; set; } = string.Empty;  // وصف حالة الجهاز

        public DeviceStatus Status { get; set; } = DeviceStatus.Received;

        // Technician Report Data
        public string? TechnicalReport { get; set; } // تقرير الفني
        public decimal TotalCost { get; set; } = 0;

        // Assignments
        public string? ReceptionistId { get; set; }
        public virtual ApplicationUser? Receptionist { get; set; }

        public int? WorkshopId { get; set; }
        public virtual Workshop? Workshop { get; set; }

        public string? TechnicianId { get; set; }
        public virtual ApplicationUser? Technician { get; set; }

        // Navigation
        public virtual PaymentReceipt? PaymentReceipt { get; set; }
        public virtual ICollection<RequestSparePart> SpareParts { get; set; } = new List<RequestSparePart>();
        public virtual ICollection<RequestService> Services { get; set; } = new List<RequestService>();
    }
}
