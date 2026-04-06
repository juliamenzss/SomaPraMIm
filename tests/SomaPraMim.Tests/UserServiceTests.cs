using Moq;
using Moq.EntityFrameworkCore;
using SomaPraMim.Application.Services.UserServices;
using SomaPraMim.Communication.Requests.UserRequests;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;


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
                Id = id,
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

        [Fact(DisplayName = "007 - Deve retornar null quando usuário não existir")]
        public async Task GivenGetUser_WhenUserDoesNotExist_ThenReturnNull()
        {
            var context = new Mock<IUserContext>();

            context.Setup(x => x.Users).ReturnsDbSet([]);

            var service = new UserService(context.Object);
            var result = await service.GetUser(999);

            Assert.Null(result);
        }

        [Fact(DisplayName = "008 - Deve retornar false se usuário não existir")]
        public async Task GivenExists_WhenUserDoesNotExist_ThenReturnFalse()
        {
            var context = new Mock<IUserContext>();
            context.Setup(x => x.Users).ReturnsDbSet([]);
            var result = await new UserService(context.Object).Exists(999);

            Assert.False(result);
        }

        [Fact(DisplayName = "009 - Deve criar usuário corretamente")]
        public async Task GivenCreate_WhenValidRequest_ThenAddUser()
        {
            var context = new Mock<IUserContext>();

            var users = new List<User>();
            context.Setup(x => x.Users).ReturnsDbSet(users);

            var service = new UserService(context.Object);

            var request = new UserCreateRequest
            {
                Name = "Teste",
                Email = "teste@email.com",
                Password = "123"
            };

            var result = await service.Create(request);

            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Email, result.Email);
        }

        [Fact(DisplayName = "010 - Deve retornar null ao atualizar usuário inexistente")]
        public async Task GivenUpdate_WhenUserNotFound_ThenReturnNull()
        {
            var context = new Mock<IUserContext>();

            context.Setup(x => x.Users)
                   .ReturnsDbSet([]);

            var service = new UserService(context.Object);

            var request = new UserUpdateRequest
            {
                Name = "Novo Nome",
                Password = "NovaSenha"
            };

            var result = await service.Update(request, 999);

            Assert.Null(result);
        }

        [Fact(DisplayName = "011 - Deve retornar lista vazia quando não houver usuários")]
        public async Task GivenGetAll_WhenNoUsers_ThenReturnEmptyList()
        {
            var context = new Mock<IUserContext>();

            context.Setup(x => x.Users)
                   .ReturnsDbSet([]);

            var service = new UserService(context.Object);

            var search = new UserSearch
            {
                Page = 1,
                Size = 10,
                Term = null
            };

            var result = await service.GetAll(search);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalItems);
        }
    }
}