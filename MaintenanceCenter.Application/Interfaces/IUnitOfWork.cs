using MaintenanceCenter.Domain.Entities;

namespace MaintenanceCenter.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Expose repositories for our aggregates
        IGenericRepository<MaintenanceRequest> MaintenanceRequests { get; }
        IGenericRepository<Workshop> Workshops { get; }
        IGenericRepository<SparePart> SpareParts { get; }
        IGenericRepository<MaintenanceService> MaintenanceServices { get; }
        IGenericRepository<PaymentReceipt> PaymentReceipts { get; }

        Task<int> CompleteAsync();
    }
}