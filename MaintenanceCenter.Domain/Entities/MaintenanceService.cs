using MaintenanceCenter.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Entities
{
    public class MaintenanceService : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }
}
