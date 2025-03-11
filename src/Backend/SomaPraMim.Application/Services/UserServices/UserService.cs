using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SomaPraMim.Application.Validators;
using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Communication.Responses;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Application.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserContext _context;
        private readonly IValidator<UserCreateRequest> _createValidator;
        private readonly IValidator<UserUpdateRequest> _updateValidator;
        public UserService(
            IUserContext context,
            IValidator<UserCreateRequest> createValidator,
            IValidator<UserUpdateRequest> updateValidator)

        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<UserResponse>> GetAll(int page = 1, int pageSize = 10)
        {
            var users = await _context.Users
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
            return users;
        }
        public async Task<int> GetTotal()
        {
            return await _context.Users.CountAsync();
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
            await ValidateCreate(request);

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
            await ValidateUpdate(request, id);

            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(prop => prop
                    .SetProperty(u => u.Name, request.Name)
                    .SetProperty(u => u.Password, request.Password));

            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Id == id);

            return user!;
        }


        public async Task Delete(long id)
        {
            await _context
                .Users.Where(x => x.Id == id)
                .ExecuteDeleteAsync();
        }


        private async Task ValidateCreate(UserCreateRequest request)
        {
            var result = _createValidator.Validate(request);

            var emailExist = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (emailExist)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "E-mail já em uso."));
            }

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join("; ", errorMessages));
            }
        }

        private async Task ValidateUpdate(UserUpdateRequest request, long id)
        {
            var result = _updateValidator.Validate(request);

            var userExists = await _context.Users.AnyAsync(u => u.Id == id);
            if (!userExists)
            {
                throw new KeyNotFoundException($"Usuário com Id não encontrado.");
            }

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
                throw new ArgumentException(string.Join("; ", errorMessages));
            }
        }
    }
}
