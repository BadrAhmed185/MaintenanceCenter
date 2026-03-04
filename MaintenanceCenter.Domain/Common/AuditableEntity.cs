namespace MaintenanceCenter.Domain.Common
{
    public abstract class AuditableEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedById { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedById { get; set; }
    }
}