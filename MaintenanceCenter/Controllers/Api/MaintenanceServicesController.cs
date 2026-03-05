using MaintenanceCenter.Application.DTOs.MaintenanceServices;
using MaintenanceCenter.Application.DTOs.SpareParts;
using MaintenanceCenter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Api
{
    // [Authorize(Roles = "Admin")] 
    public class MaintenanceServicesController : BaseApiController
    {
        private readonly IMaintenanceService _maintenanceServices;

        public MaintenanceServicesController(IMaintenanceService maintenanceService)
        {
            _maintenanceServices = maintenanceService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _maintenanceServices.GetAllAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _maintenanceServices.GetByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateMaintenanceServiceDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _maintenanceServices.CreateAsync(dto);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateMaintenanceServiceDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            if (id != dto.Id)
                return BadRequest(new { Message = "ID mismatch" });

            var result = await _maintenanceServices.UpdateAsync(dto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _maintenanceServices.DeleteAsync(id);
            return HandleResult(result);
        }
    }
}