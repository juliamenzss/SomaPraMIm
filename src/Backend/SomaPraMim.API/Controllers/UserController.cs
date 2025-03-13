using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using SomaPraMim.Application.Services.UserServices;
using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Communication.Responses;

namespace SomaPraMim.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService service) : ControllerBase
    {
        private readonly IUserService _service = service ?? throw new ArgumentNullException(nameof(service));
        
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<PaginateResponse<UserResponse>>> GetUsers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? searchTerm = null)
        {
            var paginatedResult = await _service.GetAll(page, pageSize, searchTerm);
            return Ok(paginatedResult);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var exists = await _service.Exists(id);
            if(exists){
            var user = await _service.GetUser(id);
            return Ok(user);
            }
            return NotFound();
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> PostUser(UserCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                
                var user = await _service.Create(request);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (DbException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(UserUpdateRequest request, long id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _service.Exists(id))
                return NotFound();

            try
            {
                var user = await _service.Update(request, id);
                return user == null ? NotFound() : NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "Usuário não encontrado." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Erro interno no servidor." });
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        public async Task<IActionResult> DeleteUser(long id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}