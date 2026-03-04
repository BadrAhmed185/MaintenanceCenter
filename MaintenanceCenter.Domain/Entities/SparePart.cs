using MaintenanceCenter.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Entities
{
    // 4. Catalogs (قطع الغيار والخدمات)
    public class SparePart : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }
}
