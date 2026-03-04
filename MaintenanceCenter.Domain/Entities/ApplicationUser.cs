using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain.Entities
{
    // 1. Extend Identity User to fit your needs
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        // If the user is a Technician, they are assigned to a Workshop
        public int? WorkshopId { get; set; }
        public virtual Workshop? Workshop { get; set; }
    }
}
