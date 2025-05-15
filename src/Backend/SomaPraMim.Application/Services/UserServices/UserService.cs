using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Communication.Responses;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;
using System.Linq;

namespace SomaPraMim.Application.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserContext _context;
        public UserService(
            IUserContext context)

        {
            _context = context;
        }

        public async Task<PaginateResponse<UserResponse>> GetAll(UserSearch search)
        {
            var query = _context.Users
                .Where(UserFilters.SearchByName(search));

            var totalItems = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Name)
                .Skip((search.Page - 1) * search.Size)
                .Take(search.Size)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                })
                .ToListAsync(); 

            return new PaginateResponse<UserResponse>
            {
                Items = users,
                CurrentPage = search.Page,
                PageSize = search.Size,
                TotalItems = totalItems
            };
        }


        public async Task<UserResponse> GetUser(long id)
        {
            var query = _context.Users
                .Where(x => x.Id == id)
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email
                });

                var user = await query.SingleOrDefaultAsync();
                return user!;
        }

        public async Task<User> Create(UserCreateRequest request)
        {

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<User> Update(UserUpdateRequest request, long id)
        {
             var user = await _context.Users
                    .SingleOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return null!;
            }

            user.Name = request.Name;
            user.Password = request.Password;

            await _context.SaveChangesAsync();
            return user;
        }


        public async Task Delete(long id)
        {
            await _context
                .Users.Where(x => x.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(long id)
        {
            return await _context.Users
                .AnyAsync(x => x.Id == id);
        }
        
    }
}
