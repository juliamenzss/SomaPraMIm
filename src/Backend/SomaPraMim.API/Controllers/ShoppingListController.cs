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

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ShoppingItemResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{shoppingListId}/items")]
        public async Task<IActionResult> GetItemsByShoppingList(long shoppingListId)
        {
            var items = await _service.GetItemsByShoppingListId(shoppingListId);

            if (items == null || !items.Any())
                return NotFound("Não há itens nessa lista ainda!");

            return Ok(items);
        }


        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ShoppingListResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpPut("{Id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(long id, ShoppingListUpdateRequest request){
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var shoppingList = await _service.UpdateShoppingList(id, request);

                if (shoppingList == null)
                {
                    return NotFound();
                }

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


        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteShoppingList(id);
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}/total")]
        public async Task<IActionResult> GetTotal(long shoppingListId)
        {
            var total = await _service.GetShoppingListTotal(shoppingListId);
            if(total <= 0){
                return NotFound("Lista vazia!");
            }
            
            return Ok(new { TotalPrice = total });
        }
    
    }
}