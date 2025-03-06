using SomaPraMim.Communication.Requests.ShoppingListRequests;
using SomaPraMim.Domain.Entities;
using SomaPraMim.Communication.Responses;


namespace SomaPraMim.Application.Services.ShoppingListServices
{
    public interface IShoppingListService
    {
        Task<ShoppingListResponse> CreateShoppingList(ShoppingListCreateRequest request);
        Task<ShoppingListResponse?> GetShoppingListById(long id);
        Task<IEnumerable<ShoppingListResponse>> GetShoppingList(int page = 1, int pageSize = 10);
        Task<int> GetTotal();
        Task<ShoppingListResponse> DeleteShoppingList(long id);
    }
}