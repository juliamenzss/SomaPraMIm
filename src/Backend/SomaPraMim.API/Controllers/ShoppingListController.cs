using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using SomaPraMim.Application.Services.ShoppingListServices;
using SomaPraMim.Communication.Requests.ShoppingListRequests;
using SomaPraMim.Domain.Entities;

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
                return CreatedAtAction(nameof(GetList), new { id = shoppingList.Id }, shoppingList);
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

        private object GetList()
        {
            throw new NotImplementedException();
        }
    }
}