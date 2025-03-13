using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Communication.Responses;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Application.Services.UserServices
{
    public interface IUserService
    {
        Task<PaginateResponse<UserResponse>> GetAll(int page = 1, int pageSize = 10, string? searchTerm = null);
        Task<UserResponse> GetUser(long id);
        Task<User> Create(UserCreateRequest request);
        Task<User> Update(UserUpdateRequest request, long id );
        Task Delete(long id);
        Task<bool> Exists(long id);
    }
}