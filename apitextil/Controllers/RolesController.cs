using apitextil.DTOs;
using apitextil.Models;
using apitextil.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "SuperAdmin")]  // ❌ Desactivado temporalmente para entorno local
    public class RolesController : ControllerBase
    {
        private readonly iRoleService _roleService;

        public RolesController(iRoleService roleService)
        {
            _roleService = roleService;
        }

        // ✅ Obtener todos los roles (sin token)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        // ✅ Obtener un rol por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetById(int id)
        {
            var rol = await _roleService.GetByIdAsync(id);
            if (rol == null) return NotFound();
            return Ok(rol);
        }

        // ✅ Crear un nuevo rol
        [HttpPost]
        public async Task<ActionResult<Rol>> Create([FromBody] CreateRolDto dto)
        {
            // Simulamos un adminId fijo (en producción se obtiene del token JWT)
            var adminId = 4;
            var rol = await _roleService.CreateAsync(dto, adminId);
            return CreatedAtAction(nameof(GetById), new { id = rol.Id }, rol);
        }

        // ✅ Actualizar rol existente
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateRolDto dto)
        {
            var success = await _roleService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        // ✅ Eliminar rol
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _roleService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
