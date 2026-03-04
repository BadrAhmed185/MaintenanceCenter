using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Entities
{
    public class RequestService
    {
        public int MaintenanceRequestId { get; set; }
        public virtual MaintenanceRequest MaintenanceRequest { get; set; } = null!;

        public int MaintenanceServiceId { get; set; }
        public virtual MaintenanceService MaintenanceService { get; set; } = null!;

        public decimal PriceSnapshot { get; set; } // Crucial: Saves the service cost AT THE TIME OF REPAIR
    }
}
