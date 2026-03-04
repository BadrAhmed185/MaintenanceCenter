using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Application.DTOs.MaintenanceServices
{
    public class MaintenanceServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }

    public class CreateMaintenanceServiceDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }

    public class UpdateMaintenanceServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentCost { get; set; }
    }
}
