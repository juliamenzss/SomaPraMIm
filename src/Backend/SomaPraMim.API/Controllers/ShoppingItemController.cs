using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using SomaPraMim.Application.Services.ShoppingItemServices;
using SomaPraMim.Communication.Requests.ShoppingItemRequests;
using SomaPraMim.Communication.Responses;

namespace SomaPraMim.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingItemController(IShoppingItemService service) : ControllerBase
    {
        private readonly IShoppingItemService _service = service ?? throw new ArgumentNullException(nameof(service));


        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create(ShoppingItemCreateRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var shoppingItem = await _service.CreateShoppingItem(request);
                return CreatedAtAction(nameof(GetById), new { id = shoppingItem.Id }, shoppingItem);
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
            var item = await _service.GetShoppingItemById(id);
            if (item == null) return NotFound();

            return Ok(item);
        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItem(long id)
        {
            var item = await _service.RemoveShoppingItem(id);

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ShoppingItemResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pagesize = 10)
        {
            var shoppingItems = await _service.GetShoppingItem(page, pagesize);

            if (shoppingItems == null || !shoppingItems.Any())
            {
                return NotFound();
            }

            var result = new PaginateResponse<ShoppingItemResponse>
            {
                CurrentPage = page,
                PageSize = pagesize,
                TotalItems = await _service.GetTotal(),
                Items = shoppingItems,
            };
            return Ok(result);
        }

    }
}
