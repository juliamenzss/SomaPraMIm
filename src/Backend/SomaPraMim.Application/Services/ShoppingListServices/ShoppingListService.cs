using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;
using SomaPraMim.Communication.Requests.ShoppingListRequests;
using SomaPraMim.Communication.Responses;
using Microsoft.EntityFrameworkCore;


namespace SomaPraMim.Application.Services.ShoppingListServices
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListContext _context;

        public ShoppingListService(IShoppingListContext context)
        {
            _context = context;
        }



        public async Task<ShoppingList> CreateShoppingList(ShoppingListCreateRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var newList = new ShoppingList
            {
                Name = request.Name,
                MarketName = request.MarketName,
                Budget = request.Budget,
                UserId = request.UserId,
            };

            _context.ShoppingLists.Add(newList);
            await _context.SaveChangesAsync();
            return newList;
        }

        public async Task<ShoppingListResponse?> GetShoppingListById(long id)
        {
            var shopppingList = await _context.ShoppingLists
                .Where(x => x.Id == id)
                .Select(x => new ShoppingListResponse
                {
                    Name = x.Name

                })
                .SingleOrDefaultAsync();

            if (shopppingList == null) return null!;

            return shopppingList;
        }
    }
}