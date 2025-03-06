using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using SomaPraMim.Application.Services.ShoppingListServices;
using SomaPraMim.Communication.Requests.ShoppingListRequests;
using SomaPraMim.Communication.Responses;

namespace SomaPraMim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingListController(IShoppingListService service) : ControllerBase
    {
        private readonly IShoppingListService _service = service ?? throw new ArgumentNullException(nameof(service));


        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create(ShoppingListCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);


                var shoppingList = await _service.CreateShoppingList(request);
                return CreatedAtAction(nameof(GetById), new { id = shoppingList.Id }, shoppingList);
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var shoppingList = await _service.GetShoppingListById(id);
            if (shoppingList == null) return NotFound();
            
            return Ok(shoppingList);
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ShoppingListResponse>))]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pagesize = 10)
        {
            var shoppingLists = await _service.GetShoppingList(page, pagesize);
            if(shoppingLists == null || !shoppingLists.Any()){
                return NotFound();
            }

            var result = new PaginateResponse<ShoppingListResponse>
            {
                CurrentPage = page,
                PageSize = pagesize,
                TotalItems = await _service.GetTotal(),
                Items = shoppingLists,
            };
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteShoppingList(id);
            return Ok();
        }
    }
}