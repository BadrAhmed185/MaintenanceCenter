using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Enums
{
    public enum DeviceStatus
    {
        Received = 1,           // تم الاستلام (Receptionist)
        UnderInspection = 2,    // تحت الفحص (Assigned to Tech)
        QuotationReady = 3,     // المقايسة جاهزة (Tech submitted report)
        QuotationRejected = 4,  // رفض المقايسة (Client refused cost - Edge Case)
        Unrepairable = 5,       // غير قابل للإصلاح (No parts / Too expensive)
        UnderRepair = 6,        // جاري التصليح (Payment verified, work started)
        ReadyForDelivery = 7,   // جاهز للاستلام (Repair done OR Unrepairable/Rejected and waiting for pickup)
        Delivered = 8           // تم التسليم (Closed)
    }
}
