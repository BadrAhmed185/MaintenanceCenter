using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain.Common;
using MaintenanceCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceCenter.Domain
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Workshop> Workshops { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<MaintenanceService> MaintenanceServices { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Critical for Identity tables

            // 1. Global Query Filters (Soft Delete)
            builder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);
            builder.Entity<Workshop>().HasQueryFilter(w => !w.IsDeleted);
            builder.Entity<MaintenanceRequest>().HasQueryFilter(m => !m.IsDeleted);
            builder.Entity<SparePart>().HasQueryFilter(s => !s.IsDeleted);
            builder.Entity<MaintenanceService>().HasQueryFilter(s => !s.IsDeleted);
            builder.Entity<PaymentReceipt>().HasQueryFilter(p => !p.IsDeleted);

            // 2. Decimal Precision (Critical for Financial Accuracy)
            builder.Entity<SparePart>()
                .Property(s => s.CurrentCost).HasColumnType("decimal(18,2)");

            builder.Entity<MaintenanceService>()
                .Property(m => m.CurrentCost).HasColumnType("decimal(18,2)");

            builder.Entity<MaintenanceRequest>()
                .Property(m => m.TotalCost).HasColumnType("decimal(18,2)");

            // 3. Composite Keys for Join Tables
            builder.Entity<RequestSparePart>()
                .HasKey(rsp => new { rsp.MaintenanceRequestId, rsp.SparePartId });

            builder.Entity<RequestSparePart>()
                .Property(rsp => rsp.UnitPriceSnapshot).HasColumnType("decimal(18,2)");

            builder.Entity<RequestService>()
                .HasKey(rs => new { rs.MaintenanceRequestId, rs.MaintenanceServiceId });

            builder.Entity<RequestService>()
                .Property(rs => rs.PriceSnapshot).HasColumnType("decimal(18,2)");

            // 4. One-to-One: MaintenanceRequest <-> PaymentReceipt
            builder.Entity<MaintenanceRequest>()
                .HasOne(m => m.PaymentReceipt)
                .WithOne(p => p.MaintenanceRequest)
                .HasForeignKey<PaymentReceipt>(p => p.MaintenanceRequestId)
                .OnDelete(DeleteBehavior.Cascade); // If request is hard-deleted (rare), cascade. Otherwise soft-delete protects it.

            // 5. Explicit Foreign Keys to Prevent Multiple Cascade Paths
            builder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Receptionist)
                .WithMany()
                .HasForeignKey(m => m.ReceptionistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Technician)
                .WithMany()
                .HasForeignKey(m => m.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Workshop)
                .WithMany(w => w.MaintenanceRequests)
                .HasForeignKey(m => m.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // 6. Automated Auditing and Soft Delete Enforcement
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var userId = _currentUserService.UserId;
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedById = userId;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedById = userId;
                        break;

                    case EntityState.Deleted:
                        // Intercept hard deletes and convert them to soft deletes
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedById = userId;
                        break;
                }
            }

            // Handle ApplicationUser soft delete explicitly since it doesn't inherit AuditableEntity
            foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
