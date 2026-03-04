using System.Security.Claims;
using MaintenanceCenter.Application.Interfaces;

namespace MaintenanceCenter.Web.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            string NameIdentifier = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(NameIdentifier);
            string id = _httpContextAccessor.HttpContext?.User?.FindFirstValue("NameIdentifier");
            Console.WriteLine(id);
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
    }
}