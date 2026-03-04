using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Entities
{
    // 5. Join Tables (With Price Snapshots!)
    public class RequestSparePart
    {
        public int MaintenanceRequestId { get; set; }
        public virtual MaintenanceRequest MaintenanceRequest { get; set; } = null!;

        public int SparePartId { get; set; }
        public virtual SparePart SparePart { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPriceSnapshot { get; set; } // Crucial: Saves the price AT THE TIME OF REPAIR
    }
}
