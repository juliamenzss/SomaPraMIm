using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;
using SomaPraMim.Communication.Requests.ShoppingListRequests;


namespace SomaPraMim.Application.Services.ShoppingListServices
{
    public class ShoppingListService(IShoppingListContext context) : IShoppingListService
    {
        private readonly IShoppingListContext _context = context;


        public async Task<ShoppingList> CreateShoppingList(ShoppingListCreateRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);

            if(user == null)
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
            await context.SaveChangesAsync();
            return newList;
        }
    }
    
}