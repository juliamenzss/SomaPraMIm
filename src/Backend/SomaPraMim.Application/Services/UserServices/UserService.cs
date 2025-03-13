using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Communication.Responses;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;

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

        public async Task<PaginateResponse<UserResponse>> GetAll(int page = 1, int pageSize = 10, string? searchTerm = null)
        {
            var query = _context.Users.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x => EF.Functions.Like(x.Name.ToLower(), $"%{searchTerm.ToLower()}%"));
            }

            var totalItems = await query.CountAsync();

            var users = await query
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                })
                .ToListAsync();  
                
            return new PaginateResponse<UserResponse>
            {
                Items = users,
                CurrentPage = page,
                PageSize = pageSize,
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
