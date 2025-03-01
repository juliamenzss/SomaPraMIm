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
        public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 10)
        {
            var users = await _service.GetAll(page, pageSize);

            if (users == null || !users.Any())
            {
                return NotFound("Users not found");
            }

            var result = new PaginateResponse<UserResponse>
            {
                CurrentPage = page,
                PageSize = pageSize,
                Items = users,
                TotalItems = await _service.GetTotal()
            };
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var user = await _service.GetUserById(id);
            if(user == null) return NotFound();
            
            return Ok(user);
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
        public async Task<IActionResult> PutUser(long id, UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _service.Update(id, request);
                if(user == null) return NotFound();

                return NoContent();
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
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        public async Task<IActionResult> DeleteUser(long id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}