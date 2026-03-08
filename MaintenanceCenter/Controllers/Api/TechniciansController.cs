using MaintenanceCenter.Application.DTOs.Technicians;
using MaintenanceCenter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Api
{
     [Authorize(Roles = "Admin")] 
    public class TechniciansController : BaseApiController
    {
        private readonly ITechnicianService _technicianService;

        public TechniciansController(ITechnicianService technicianService)
        {
            _technicianService = technicianService;
        }

        [HttpGet]

        public async Task<ActionResult> GetAll()
        {
            var result = await _technicianService.GetAllAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(string id)
        {
            var result = await _technicianService.GetByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateTechnicianDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _technicianService.CreateAsync(dto);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] UpdateTechnicianDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            if (id != dto.Id)
                return BadRequest(new { Message = "ID mismatch" });

            var result = await _technicianService.UpdateAsync(dto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var result = await _technicianService.DeleteAsync(id);
            return HandleResult(result);
        }
    }
}