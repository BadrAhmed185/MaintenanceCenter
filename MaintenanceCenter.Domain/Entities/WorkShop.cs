using MaintenanceCenter.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Entities
{
    // 2. Workshops (الورش)
    public class Workshop : AuditableEntity
    {
        public string Name { get; set; } = string.Empty; // e.g., ورشة الإلكترونيات الدقيقة
        public string? Description { get; set; }

        public virtual ICollection<ApplicationUser> Technicians { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}
