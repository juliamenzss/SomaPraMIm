using SomaPraMim.Communication.Requests.ShoppingListRequests;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Application.Services.ShoppingListServices
{
    public interface IShoppingListService
    {
        Task<ShoppingList> CreateList(ShoppingListCreateRequest request);
    }
}