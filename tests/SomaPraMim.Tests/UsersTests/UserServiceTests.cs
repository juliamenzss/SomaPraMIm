using Moq;
using Moq.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SomaPraMim.Domain.Entities;
using SomaPraMim.Application.Services.UserServices;
using SomaPraMim.Communication.Requests.UserRequests;
using FluentValidation;
using SomaPraMim.Domain.Contexts;


namespace SomaPraMim.Tests.UsersTests
{
    public class UserServiceTests
    {
        [Fact(DisplayName = "001 Retorna lista de usuários paginada")]
        public async Task GetAll_WithPagination_ReturnsPaginatedUserList()
        {
            var users = new List<User>
            {
                new (){  Id = Random.Shared.Next(), Name = Guid.NewGuid().ToString(), Email = Guid.NewGuid().ToString() },
                new (){  Id = Random.Shared.Next(), Name = Guid.NewGuid().ToString(), Email = Guid.NewGuid().ToString() },
            };
        
            var context = new Mock<IUserContext>();

            context.
                Setup(x => x.Users)
                .ReturnsDbSet(users);

            var service = new UserService(
                context.Object);

            var search = new UserSearch
            {
                Page = 1,
                Size = 10,
                Term = null
            };

            var result = await service.GetAll(search);

            Assert.Equal(search.Page, result.CurrentPage);
            Assert.Equal(search.Size, result.PageSize);
            Assert.Equal(users.Count, result.TotalItems);
        }

        [Fact(DisplayName = "002 Retorna search term")]
        public async Task GetAll_WithSearchTerm_ReturnsFilteredUserList()
        {
            var users = new List<User>
            {
                new() { Id = 1, Name = "Alice Teste", Email = "alice@email.com" },
                new() { Id = 2, Name = "Bob Silva", Email = "bob@email.com" }
            };

            var search = new UserSearch
            {
                Page = 1,
                Size = 10,
                Term = "Teste"
            };

            var context = new Mock<IUserContext>();

            context.Setup(x => x.Users)
                   .ReturnsDbSet(users);

            var service = new UserService(
                context.Object);

            var result = await service.GetAll(search);

            Assert.Single(result.Items);
            Assert.Equal("Alice Teste", result.Items.First().Name);
        }

        [Fact(DisplayName = "003 Retorna get de usuário")]
        public async Task GivenGet_ThenReturnsUser()
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
                context.Object);

            var user = await service.GetUser(users[0].Id);

            Assert.Equal(users[0].Name, user.Name);
        }


        [Fact(DisplayName = "004 - Deve retornar true se existir")]
        public async Task GivenExists_ThenReturnTrue()
        {
            var user = new User()
            {
                Id = Random.Shared.Next(),
                Name = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
            };
            var context = new Mock<IUserContext>();

            context.
                Setup(x => x.Users)
                .ReturnsDbSet([user]);

            var service = new UserService(
                context.Object);

            var exixts = await service.Exists(user.Id);

            Assert.True(exixts);
        }

        [Fact(DisplayName = "005 - Deve chamar savechange uma vez")]
        public async Task GivenCreate_ThenReturnCallSaveChangeAsync()
        {
            var user = new UserCreateRequest()
            {
                Name = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
            };

            var context = new Mock<IUserContext>();

            context.
                Setup(x => x.Users)
                .ReturnsDbSet([]);

            var service = new UserService(
                context.Object);

            await service.Create(user);
            
            context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "006 - Deve fazer update")]
        public async Task GivenUpdate_ThenReturn()
        {
            var id = Random.Shared.Next();

            var user = new User()
            {
                Id =  id,
                Name = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
            };

             var userUpdate = new UserUpdateRequest()
            {
                Name = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString(),
            };

            var users = new List<User> { user };
            var context = new Mock<IUserContext>();
            context
                .Setup(x => x.Users)
                .ReturnsDbSet(users);

            var service = new UserService(context.Object);

            await service.Update(userUpdate, id);
            context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}