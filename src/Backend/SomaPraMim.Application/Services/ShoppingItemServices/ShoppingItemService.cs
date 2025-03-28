using SomaPraMim.Domain.Entities;
using SomaPraMim.Communication.Requests.ShoppingItemRequests;
using SomaPraMim.Communication.Responses;
using SomaPraMim.Domain.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SomaPraMim.Application.Services.ShoppingItemServices
{
    public class ShoppingItemService : IShoppingItemService
    {
        private readonly IShoppingItemContext _context;

        public ShoppingItemService(IShoppingItemContext context){
            _context = context;
        }

        public async Task<ShoppingItemResponse> CreateShoppingItem(ShoppingItemCreateRequest request)
        {
            var existingList = await _context.ShoppingLists.FindAsync(request.ShoppingListId);
            if (existingList == null)
            {
                throw new Exception("A lista de compras especificada não existe.");
            }

            var shoppingItem = new ShoppingItem
            {
                Name = request.Name,
                Quantity = request.Quantity,
                Price = request.Price,
                Unit = request.Unit,
                ShoppingListId = request.ShoppingListId
            };
            _context.ShoppingItems.Add(shoppingItem);
            await _context.SaveChangesAsync();

            return new ShoppingItemResponse
            {
                Id = shoppingItem.Id,
                Name = shoppingItem.Name,
                Quantity = shoppingItem.Quantity,
                Price = shoppingItem.Price,
            };
        }

          public async Task<ShoppingItemResponse> GetShoppingItemById(long id)
          {
        
            var item = await _context.ShoppingItems 
                .Where(x => x.Id == id)
                .Select(x => new ShoppingItemResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Price = x.Price,
                })
                .SingleOrDefaultAsync();


            if(item == null) return null!;
            return item;
        }
        public async Task<IEnumerable<ShoppingItemResponse>> GetShoppingItem(int page = 1, int pageSize = 10)
        {
            var items = await _context.ShoppingItems
            .OrderBy(x => x.Name)
            .Select( x => new ShoppingItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                Quantity = x.Quantity,
                Price = x.Price,
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return items;
        }
        public async Task<int> GetTotal()
        {
            return await _context.ShoppingItems.CountAsync();
        }

        
    }
}