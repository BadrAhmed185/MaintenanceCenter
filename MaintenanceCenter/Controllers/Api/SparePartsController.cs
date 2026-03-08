using MaintenanceCenter.Application.DTOs.SpareParts;
using MaintenanceCenter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceCenter.Web.Controllers.Api
{
     [Authorize] 
    public class SparePartsController : BaseApiController
    {
        private readonly ISparePartService _sparePartService;

        public SparePartsController(ISparePartService sparePartService)
        {
            _sparePartService = sparePartService;
        }

        [HttpGet]
   
        public async Task<ActionResult> GetAll()
        {
            var result = await _sparePartService.GetAllAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
     
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _sparePartService.GetByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> Create([FromBody] CreateSparePartDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            var result = await _sparePartService.CreateAsync(dto);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> Update(int id, [FromBody] UpdateSparePartDto dto)
        {
            if (!TryValidate(out var error)) return error!;

            if (id != dto.Id)
                return BadRequest(new { Message = "ID mismatch" });

            var result = await _sparePartService.UpdateAsync(dto);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> Delete(int id)
        {
            var result = await _sparePartService.DeleteAsync(id);
            return HandleResult(result);
        }
    }
}