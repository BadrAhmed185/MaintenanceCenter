using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,            // قيد المراجعة
        Verified = 2,           // تم التأكيد (Admin approved)
        Rejected = 3            // مرفوض (Invalid receipt uploaded - Edge Case)
    }
}
