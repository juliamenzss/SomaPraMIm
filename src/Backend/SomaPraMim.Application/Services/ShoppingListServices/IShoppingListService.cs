using SomaPraMim.Communication.Requests.ShoppingListRequests;
using SomaPraMim.Domain.Entities;
using SomaPraMim.Communication.Responses;


namespace SomaPraMim.Application.Services.ShoppingListServices
{
    public interface IShoppingListService
    {
        Task<ShoppingList> CreateShoppingList(ShoppingListCreateRequest request);
        Task<ShoppingListResponse?> GetShoppingListById(long id);
    }
}