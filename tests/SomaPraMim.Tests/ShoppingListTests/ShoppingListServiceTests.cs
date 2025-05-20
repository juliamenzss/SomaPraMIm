using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using SomaPraMim.Application.Services.ShoppingListServices;
using SomaPraMim.Communication.Requests.ShoppingListRequests;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;


namespace SomaPraMim.Tests.ShoppingListTests
{
    public class ShoppingListServiceTests
    {
        private readonly Mock<IShoppingListContext> _context;
        private readonly ShoppingListService _service;

        public ShoppingListServiceTests(){
            _context = new Mock<IShoppingListContext>();
            _service = new ShoppingListService(_context.Object);
        }
        [Fact(DisplayName = "001 - Deve retornar valor total")]
        public async Task GivenShoppingListWhenGetTotalThenReturnsCorrectSum()
        {
            var shoppingListId = Random.Shared.Next(0, 100);

            var fakeShoppingItem = new List<ShoppingItem>
            {
                new () {ShoppingListId = shoppingListId, Price = 15m, Quantity = 2},
                new () {ShoppingListId = shoppingListId, Price = 2.5m, Quantity = 4},
            };

            _context
                .Setup(x => x.ShoppingItems)
                .ReturnsDbSet(fakeShoppingItem);

            var expectResult = await _service.GetItemsByShoppingListId(shoppingListId);
            var total = expectResult.Sum(item => item!.Price * item.Quantity);

            total.Should().Be(40m);
        }
        [Fact(DisplayName = "002 - Deve retornar save changes se adicionado com dados válidos")]
        public async Task CreateShoppingList_ShouldCallAddAndSaveChanges()
        {
            var userId = 1;
            var request = new ShoppingListCreateRequest
            {
                Name = "TestList",
                Budget = 100,
                MarketName = "TestMarket",
                UserId = userId
            };

            _context
                .Setup(x => x.Users.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(new User { Id = userId });  //simula q user existe no banco pq o service verifica se userId é valido antes de criar lista

            var fakeList = new List<ShoppingList>(); //simula minha lista
            _context
                .Setup(x => x.ShoppingLists) //quando acessar shoppingLists
                .ReturnsDbSet(fakeList); //use/retorne esta lista fake


            _context
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _service.CreateShoppingList(request);
            _context.Verify(x => x.ShoppingLists.Add(It.IsAny<ShoppingList>()), Times.Once());
            _context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
        [Fact(DisplayName = "003 - Deve retornar Id se adicionado com dados válidos")]
        public async Task CreateShoppingList_ShouldReturnCreatedListId()
        {
            var userId = 1;
            var expectId = 123;
            var request = new ShoppingListCreateRequest
            {
                Name = "TestList",
                Budget = 100,
                MarketName = "TestMarket",
                UserId = userId
            };

            _context
                .Setup(x => x.Users.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(new User { Id = userId });

            _context
                .Setup(x => x.ShoppingLists
                .Add(It.IsAny<ShoppingList>()))// configura o mock para interceptar qualquer objeto ShoppingList passado no Add()
                .Callback<ShoppingList>(list => list.Id = expectId);// simula o EF atribuindo um ID após salvar (como se fosse o banco retornando o valor gerado)

            _context
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.CreateShoppingList(request);

            result.Id.Should().Be(expectId);
        }
        [Fact(DisplayName = "004 - Deve retornar dados completos se lista existir pelo ID")]
        public async Task GetShoppingListById_ShouldReturnFullData_WhenListExists()
        {
            var fakeShoppingList = new ShoppingList()
            {
                Id = 123,
                Name = "TestList",
                Budget = 100,
                MarketName = "TestMarket",
                UserId = 1,
                ShoppingItems = new List<ShoppingItem>
                {
                    new () { Name = "Item-1", Quantity = 2, Price = 10 },
                    new () { Name = "Item-2", Quantity = 1, Price = 15 }
                }
            };

            _context.Setup(x => x.ShoppingLists).ReturnsDbSet([fakeShoppingList]);

            var result = await _service.GetShoppingListById(fakeShoppingList.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(fakeShoppingList.Id);
            result.Name.Should().Be(fakeShoppingList.Name);
            result.Budget.Should().Be(fakeShoppingList.Budget);
            result.MarketName.Should().Be(fakeShoppingList.MarketName);
            result.UserId.Should().Be(fakeShoppingList.UserId);
            result.TotalItems.Should().Be(3);
            result.TotalPrice.Should().Be(35);
        }
        [Fact(DisplayName = "005 - Deve retornar nulo se lista não existir pelo ID")]
        public async Task GetShoppingListById_ShouldReturnNull_WhenListDoesNotExist()
        {
            var emptyShoppingList = new ShoppingList();
            var fakeId = Random.Shared.Next();
            _context.Setup(x => x.ShoppingLists).ReturnsDbSet([emptyShoppingList]);

            var result = await _service.GetShoppingListById(fakeId);
            result.Should().BeNull();
        }
        [Fact(DisplayName = "006 - Deve calcular total e quantidade se lista tiver itens")]
        public async Task GetShoppingListById_ShouldCalculateTotalPriceAndQuantity_WhenListHasItems()
        {
            var listId = 123;
            var fakeShoppingList = new List<ShoppingList>
            {
                new () {
                Id = listId,
                Name = "Test",
                Budget = 100,
                MarketName = "TestMarket",
                UserId = 1,
                ShoppingItems =
                [
                    new () { Name = "Item-1", Quantity = 2, Price = 10 },
                    new () { Name = "Item-2", Quantity = 1, Price = 15 }
                ]}
            };
            _context
              .Setup(x => x.ShoppingLists)
              .ReturnsDbSet(fakeShoppingList);

            var result = await _service.GetShoppingListById(listId);
            result.Should().NotBeNull();
            result.TotalItems.Should().Be(3);
            result.TotalPrice.Should().Be(35);
        }
        [Fact(DisplayName = "007 - Deve ignorar itens com preço zero")]
        public async Task GetShoppingListById_ShouldIgnoreItemsWithZeroPrice()
        {
            var listId = 123;
            var fakeShoppingList = new ShoppingList()
            {
                Id = listId,
                Name = "Test",
                Budget = 100,
                MarketName = "TestMarket",
                UserId = 1,
                ShoppingItems =
                [
                    new () { Name = "Item-1", Quantity = 2, Price = 0 },
                    new () { Name = "Item-1", Quantity = 2, Price = 10 },
                    new () { Name = "Item-2", Quantity = 1, Price = 15 }
                ]
            };
            _context
                .Setup(x => x.ShoppingLists)
                .ReturnsDbSet([fakeShoppingList]);
            var result = await _service.GetShoppingListById(listId);
            result.Should().NotBeNull();
            result.TotalItems.Should().Be(5);
            result.TotalPrice.Should().Be(35);
        }
        [Fact(DisplayName = "008 - Deve tratar lista nula de itens corretamente")]
        public async Task GetShoppingListById_ShouldHandleNullItemsList()
        {
            var listId = 123;
            var fakeShoppingList = new ShoppingList()
            {
                Id = listId,
                Name = "Test",
                Budget = 100,
                MarketName = "TestMarket",
                UserId = 1,
                ShoppingItems = []
            };
            _context
                .Setup(x => x.ShoppingLists)
                .ReturnsDbSet([fakeShoppingList]);

            var result = await _service.GetShoppingListById(listId);

            result.Should().NotBeNull();
            result.TotalItems.Should().Be(0);
            result.TotalPrice.Should().Be(0);
        }
        [Fact(DisplayName = "009 - Deve retornar todos os itens se lista de compras for válida")]
        public async Task GetItemsByShoppingListId_ShouldReturnAllItems_WhenShoppingListIsValid()
        {
            var listId = 123;
            var fakeItems = new List<ShoppingItem>
            {
                new () { Id=1, Name = "Pão", Quantity = 1, Price= 5.5m,  ShoppingListId = listId},
                new () { Id=2, Name = "Manteiga", Quantity = 2, Price= 10.25m,  ShoppingListId = listId}
            };

            _context.Setup(x => x.ShoppingItems).ReturnsDbSet(fakeItems);

            var result = await _service.GetItemsByShoppingListId(listId);
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Pão");
            result.Last().Name.Should().Be("Manteiga");

            _context.Verify(x => x.ShoppingItems, Times.Once);
        }
        [Fact(DisplayName = "010 - Deve retornar lista vazia se lista de compras não tiver itens")]
        public async Task GetItemsByShoppingListId_ShouldReturnEmptyList_WhenShoppingListHasNoItems()
        {
            var listId = 123;
            var fakeItems = new List<ShoppingItem>();

            _context
                .Setup(x => x.ShoppingItems)
                .ReturnsDbSet(fakeItems);

            var result = await _service.GetItemsByShoppingListId(listId);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        [Fact(DisplayName = "011 - Deve atualizar e retornar lista se ID e dados forem válidos")]
        public async Task UpdateShoppingList_ShouldUpdateAndReturnList_WhenIdAndDataAreValid()
        {
            var listId = 123;
            var shoppingList = new ShoppingList()
            {
                Id = listId,
                Name = Guid.NewGuid().ToString(),
                Budget = Random.Shared.Next(0, 1000),
                MarketName = Guid.NewGuid().ToString(),
            };
            var shoppingListUpdate = new ShoppingListUpdateRequest()
            {
                Name = Guid.NewGuid().ToString(),
                Budget = Random.Shared.Next(0, 1000),
                MarketName = Guid.NewGuid().ToString(),
            };
            _context.Setup(x => x.ShoppingLists).ReturnsDbSet([shoppingList]);

            var result = await _service.UpdateShoppingList(listId, shoppingListUpdate);

            _context
                .Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            result.Id.Should().Be(listId);
            result.Name.Should().Be(shoppingListUpdate.Name);
            result.Budget.Should().Be(shoppingListUpdate.Budget);
            result.MarketName.Should().Be(shoppingListUpdate.MarketName);

        }

        [Fact(DisplayName = "012 - Deve manter valores originais se campos da requisição forem nulos")]
        public async Task UpdateShoppingList_ShouldKeepOriginalValues_WhenRequestFieldsAreNull()
        {

            var listId = 123;
            var shoppingList = new ShoppingList()
            {
                Id = listId,
                Name = Guid.NewGuid().ToString(),
                Budget = Random.Shared.Next(0, 1000),
                MarketName = Guid.NewGuid().ToString(),
            };
            var shoppingListUpdate = new ShoppingListUpdateRequest()
            {
                Name = null,
                Budget = null,
                MarketName = null,
            };

            _context
                .Setup(x => x.ShoppingLists)
                .ReturnsDbSet([shoppingList]);

            var result = await _service.UpdateShoppingList(listId, shoppingListUpdate);

            result.Id.Should().Be(listId);
            result.Name.Should().Be(shoppingList.Name);
            result.Budget.Should().Be(shoppingList.Budget);
            result.MarketName.Should().Be(shoppingList.MarketName);
        }
        [Fact(DisplayName = "013 - Deve retornar nulo se lista de compras não existir ao atualizar")]
        public async Task UpdateShoppingList_ShouldReturnNull_WhenShoppingListDoesNotExist()
        {
            
            var shoppingListExisting = new ShoppingList { Id = 1 };
            var updateRequest = new ShoppingListUpdateRequest
            {
                Name = "Updated Name",
                Budget = 500,
                MarketName = "Updated Market"
            };
            _context.Setup(x => x.ShoppingLists).ReturnsDbSet([shoppingListExisting]);
            var result = await _service.UpdateShoppingList(999, updateRequest);

            result.Should().BeNull();
        }
    }
}