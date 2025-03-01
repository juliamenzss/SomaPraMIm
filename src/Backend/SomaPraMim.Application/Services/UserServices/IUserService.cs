using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Communication.Responses;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Application.Services.UserServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAll(int page = 1, int pageSize = 10);
        Task<int> GetTotal();
        Task<UserResponse> GetUserById(long id);
        Task<User> Create(UserCreateRequest request);
        Task<User> Update(long id, UserUpdateRequest request);
        Task<UserResponse> Delete(long id);
    }
}