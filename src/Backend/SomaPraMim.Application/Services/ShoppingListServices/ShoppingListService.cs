using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;
using SomaPraMim.Communication.Requests.ShoppingListRequests;


namespace SomaPraMim.Application.Services.ShoppingListServices
{
    public class ShoppingListService(IShoppingListContext context) : IShoppingListService
    {
        private readonly IShoppingListContext _context = context;


        public async Task<ShoppingList> CreateList(ShoppingListCreateRequest request)
        {
            var newList = new ShoppingList
            {
                Name = request.Name,
                MarketName = request.MarketName,
                Budget = request.Budget,
            };

            context.ShoppingLists.Add(newList);
            await context.SaveChangesAsync();
            return newList;
        }
    }
}