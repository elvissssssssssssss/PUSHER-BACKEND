using apitextil.DTOs;
using apitextil.Models;
using apitextil.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;


namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "SuperAdmin")]  solo el SuperAdmin puede gestionar cuentas
    public class AdminsController : ControllerBase
    {
        private readonly AdminManagementService _svc;

        public AdminsController(AdminManagementService svc)
        {
            _svc = svc;
        }

        // GET /api/Admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoginAdmin>>> GetAll()
        {
            var admins = await _svc.GetAllAsync();
            return Ok(admins);
        }

        // GET /api/Admins/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LoginAdmin>> GetById(int id)
        {
            var admin = await _svc.GetByIdAsync(id);
            if (admin == null) return NotFound();
            return Ok(admin);
        }

        // POST /api/Admins
        [HttpPost]
        public async Task<ActionResult<LoginAdmin>> Create([FromBody] CreateAdminDto dto)
        {
            try
            {
                var created = await _svc.CreateAsync(dto.Nombre, dto.Email, dto.Password, dto.RolId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH /api/Admins/{id}/toggle
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var toggled = await _svc.ToggleActiveAsync(id);
            if (toggled == null) return NotFound();

            return Ok(new
            {
                message = $"Cuenta {(toggled.Activo ? "activada" : "desactivada")} correctamente.",
                estado = toggled.Activo
            });
        }

        // DELETE /api/Admins/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _svc.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
