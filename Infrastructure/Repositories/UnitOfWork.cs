using MaintenanceCenter.Application.Interfaces;
using MaintenanceCenter.Domain;
using MaintenanceCenter.Domain.Entities;

namespace MaintenanceCenter.Infrastructure.Repositories
{
  
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lazy loading of repositories
        private IGenericRepository<MaintenanceRequest>? _maintenanceRequests;
        public IGenericRepository<MaintenanceRequest> MaintenanceRequests =>
            _maintenanceRequests ??= new GenericRepository<MaintenanceRequest>(_context);

        private IGenericRepository<Workshop>? _workshops;
        public IGenericRepository<Workshop> Workshops =>
            _workshops ??= new GenericRepository<Workshop>(_context);

        private IGenericRepository<SparePart>? _spareParts;
        public IGenericRepository<SparePart> SpareParts =>
            _spareParts ??= new GenericRepository<SparePart>(_context);

        private IGenericRepository<MaintenanceService>? _maintenanceServices;
        public IGenericRepository<MaintenanceService> MaintenanceServices =>
            _maintenanceServices ??= new GenericRepository<MaintenanceService>(_context);

        private IGenericRepository<PaymentReceipt>? _paymentReceipts;
        public IGenericRepository<PaymentReceipt> PaymentReceipts =>
            _paymentReceipts ??= new GenericRepository<PaymentReceipt>(_context);

        public async Task<int> CompleteAsync()
        {
            // This single line triggers the DbContext.SaveChangesAsync() we wrote earlier,
            // which in turn fires all our Audit/Soft Delete logic before hitting SQL.
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}