using SomaPraMim.Communication.Requests.ShoppingItemRequests;
using SomaPraMim.Communication.Responses;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Application.Services.ShoppingItemServices
{
    public interface IShoppingItemService
    {
        Task<ShoppingItemResponse> CreateShoppingItem(ShoppingItemCreateRequest request);
        Task<ShoppingItemResponse> GetShoppingItemById(long id);
        Task<PaginateResponse<ShoppingItemResponse>> GetShoppingItem(int page = 1, int pageSize = 10);
        Task<int> GetTotal();
        Task<bool> RemoveShoppingItem(long id);
    }
}