using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;
using SomaPraMim.Communication.Requests.ShoppingListRequests;
using SomaPraMim.Communication.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace SomaPraMim.Application.Services.ShoppingListServices
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListContext _context;

        public ShoppingListService(IShoppingListContext context)
        {
            _context = context;
        }

        public async Task<ShoppingListResponse> CreateShoppingList(ShoppingListCreateRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId) ?? throw new ArgumentException("User not found");
            var newList = new ShoppingList
            {
                Name = request.Name.Trim(),
                MarketName = request.MarketName.Trim(),
                Budget = request.Budget,
                UserId = request.UserId,
            };

            _context.ShoppingLists.Add(newList);
            await _context.SaveChangesAsync();

            return new ShoppingListResponse
            {
                Id = newList.Id,
            };
        }

        public async Task<ShoppingListResponse?> GetShoppingListById(long id)
        {
            var shoppingList = await _context.ShoppingLists
                .Include(x => x.ShoppingItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (shoppingList == null) return null;

            var response = new ShoppingListResponse
            {
                Id = shoppingList.Id,
                Name = shoppingList.Name,
                Budget = shoppingList.Budget,
                MarketName = shoppingList.MarketName,
                UserId = shoppingList.UserId,
                TotalItems = shoppingList.ShoppingItems?.Sum(item => item.Quantity) ?? 0,
                TotalPrice = shoppingList.ShoppingItems?.Sum(item => item.Price * item.Quantity) ?? 0
            };

            return response;
        }

        public async Task<IEnumerable<ShoppingItemResponse?>> GetItemsByShoppingListId(long shoppingListId)
        {
            var items = await _context.ShoppingItems
                .Where(x => x.ShoppingListId == shoppingListId)
                .Select(x => new ShoppingItemResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    ShoppingListId = x.ShoppingListId
                })
                .ToListAsync();

            if (items == null) return null!;
            return items;
        }

        public async Task<IEnumerable<ShoppingListResponse>> GetShoppingList(int page = 1, int pageSize = 10)
        {
            var shopppingList = await _context.ShoppingLists
            .OrderBy(x => x.Name)
            .Select( x => new ShoppingListResponse
            {
                Id = x.Id,
                Name = x.Name,
                Budget = x.Budget,
                MarketName = x.MarketName,
                UserId = x.UserId,
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            return shopppingList;
            }

        public async Task<int> GetTotal()
        {
            return await _context.ShoppingLists.CountAsync();
        }

        public async Task<ShoppingList> UpdateShoppingList(long id, ShoppingListUpdateRequest request)
        {

            var shoppingList = await _context.ShoppingLists
            .SingleOrDefaultAsync(x => x.Id == id);

            if (shoppingList == null)
            {
                return null!;
            }

            shoppingList.Name = request.Name ?? shoppingList.Name;
            shoppingList.Budget = request.Budget ?? shoppingList.Budget;
            shoppingList.MarketName = request.MarketName ?? shoppingList.MarketName;

            await _context.SaveChangesAsync();
            return shoppingList;
        }

        public async Task DeleteShoppingList(long id)
        {
            await _context.ShoppingLists
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
        }

    }
}