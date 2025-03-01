using Microsoft.EntityFrameworkCore;
using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Communication.Responses;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Application.Services.UserServices
{
    public class UserService(IUserContext  context) : IUserService
{
    public async Task<IEnumerable<UserResponse>> GetAll(int page = 1, int pageSize = 10)
    {
        var users = await context.Users
            .OrderBy(x => x.Name)
            .Select(x => new UserResponse
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return users;
    }
    public async Task<int> GetTotal()
    {
        return await context.Users.CountAsync();
    }
    
    public async Task<UserResponse> GetUserById(long id)
    {
        var user = await context.Users
            .Where(x => x.Id == id)
            .Select(x => new UserResponse
            {
                Name = x.Name,
                Email = x.Email,
            })
            .SingleOrDefaultAsync();
        
        if(user == null) return null!;
        
        return user;
    }

    public async Task<User> Create(UserCreateRequest request)
    {
        var newUser = new User
        {
            Name = request.Name,
            Email = request.Email,
        };
        context.Users.Add(newUser);
        await context.SaveChangesAsync();
        return newUser;
    }   
    
    public async Task<User> Update(long id, UserUpdateRequest request)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(x => x.Id == id);
        
        if(user == null) return null!;
        user.Name = request.Name;
        user.Email = request.Email;
        
        await context.SaveChangesAsync();
        return user;
    }
    
    public async Task<UserResponse> Delete(long id)
    {
        await context.Users.Where(x => x.Id == id).ExecuteDeleteAsync();
        return null!;
    }
    
    }
}