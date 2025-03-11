using FluentAssertions;
using SomaPraMim.Domain.Contexts;
using Moq;
using Moq.EntityFrameworkCore;
using SomaPraMim.Application.Services.UserServices;
using FluentValidation;
using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Domain.Entities;

namespace SomaPraMim.Tests.UsersTests
{
    public class UserServiceTests
    {
        [Fact(DisplayName = "001 Retorna lista de usuários")]
        public async Task GivenGetAll_ThenReturnsUserList()
        {
            var users = new List<User>
            {
                new ()
                {
                    Id = Random.Shared.Next(),
                    Name = Guid.NewGuid().ToString(),
                    Email = Guid.NewGuid().ToString(),
                },
                 new ()
                 {
                    Id = Random.Shared.Next(),
                    Name = Guid.NewGuid().ToString(),
                    Email = Guid.NewGuid().ToString(),
                },
            };
            
            var context = new Mock<IUserContext>();

            context.
                Setup(x => x.Users)
                .ReturnsDbSet(users);

            var service = new UserService(
                context.Object,
                Mock.Of<IValidator<UserCreateRequest>>(),
                Mock.Of<IValidator<UserUpdateRequest>>());

            var user = await service.GetUser(users[0].Id);

            Assert.Equal(users[0].Name, user.Name);

        }


        
    }
}