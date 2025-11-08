
// Controllers/TblEnviosController.cs
using Microsoft.AspNetCore.Mvc;
using apitextil.DTOs;
using apitextil.Services;

namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TblEnviosController : ControllerBase
    {
        private readonly ITblEnviosService _tblEnviosService;

        public TblEnviosController(ITblEnviosService tblEnviosService)
        {
            _tblEnviosService = tblEnviosService;
        }

        /// <summary>
        /// Crear un nuevo registro de envío
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TblEnviosResponseDto>> Create([FromBody] TblEnviosCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _tblEnviosService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener envío por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TblEnviosResponseDto>> GetById(int id)
        {
            try
            {
                var result = await _tblEnviosService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Registro de envío no encontrado" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener envío por ID de usuario
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<TblEnviosResponseDto>> GetByUserId(int userId)
        {
            try
            {
                var result = await _tblEnviosService.GetByUserIdAsync(userId);
                if (result == null)
                {
                    return NotFound(new { message = "Registro de envío no encontrado para este usuario" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener todos los envíos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TblEnviosResponseDto>>> GetAll()
        {
            try
            {
                var result = await _tblEnviosService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener envíos por región
        /// </summary>
        [HttpGet("region/{region}")]
        public async Task<ActionResult<IEnumerable<TblEnviosResponseDto>>> GetByRegion(string region)
        {
            try
            {
                var result = await _tblEnviosService.GetByRegionAsync(region);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtener envíos por provincia
        /// </summary>
        [HttpGet("provincia/{provincia}")]
        public async Task<ActionResult<IEnumerable<TblEnviosResponseDto>>> GetByProvincia(string provincia)
        {
            try
            {
                var result = await _tblEnviosService.GetByProvinciaAsync(provincia);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar un registro de envío
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TblEnviosResponseDto>> Update(int id, [FromBody] TblEnviosUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _tblEnviosService.UpdateAsync(id, dto);
                if (result == null)
                {
                    return NotFound(new { message = "Registro de envío no encontrado" });
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar un registro de envío
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _tblEnviosService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Registro de envío no encontrado" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Verificar si existe un DNI
        /// </summary>
        [HttpGet("exists/dni/{dni}")]
        public async Task<ActionResult<bool>> ExistsByDni(string dni)
        {
            try
            {
                var exists = await _tblEnviosService.ExistsByDniAsync(dni);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Verificar si existe un usuario
        /// </summary>
        [HttpGet("exists/user/{userId}")]
        public async Task<ActionResult<bool>> ExistsByUserId(int userId)
        {
            try
            {
                var exists = await _tblEnviosService.ExistsByUserIdAsync(userId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}