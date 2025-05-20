using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using SomaPraMim.Application.Services.ShoppingItemServices;
using SomaPraMim.Application.Services.ShoppingListServices;
using SomaPraMim.Communication.Requests.ShoppingItemRequests;
using SomaPraMim.Domain.Contexts;
using SomaPraMim.Domain.Entities;
using System.Collections.Generic;

namespace SomaPraMim.Tests.ShoppingItemTests
{
    public class ShoppingItemServiceTests
    {
        private readonly Mock<IShoppingItemContext> _context;
        private readonly ShoppingItemService _service;

        public ShoppingItemServiceTests()
        {
            _context = new Mock<IShoppingItemContext>();
            _service = new ShoppingItemService(_context.Object);
        }

        [Fact(DisplayName = "001 - Deve criar item de compra com dados válidos e atualizar lista")]
        public async Task CreateShoppingItem_ShouldCreateItemAndUpdateList_WhenDataIsValid()
        {
            var listId = Random.Shared.Next();

            var shoppingList = new ShoppingList
            {
                Id = listId,
                Budget = 100,
                TotalPrice = 10,
                TotalItems = 1
            };
            var createRequest = new ShoppingItemCreateRequest
            {
                Name = Guid.NewGuid().ToString(),
                Quantity = 10,
                Price = 20,
                ShoppingListId = listId,
            };

            _context
                .Setup(x => x.ShoppingLists
                .FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(shoppingList);

            _context
                .Setup(x => x.ShoppingItems)
                .ReturnsDbSet([]);

            _context
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.CreateShoppingItem(createRequest);

            _context
                .Verify(x => x.ShoppingItems
                .Add(It.IsAny<ShoppingItem>()), Times.Once);
            _context
                .Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact(DisplayName = "002 - Deve lançar exceção se a lista de compras não existir")]
        public async Task CreateShoppingItem_ShouldThrowException_WhenShoppingListDoesNotExist()
        {
            var listId = Random.Shared.Next();
            var createRequest = new ShoppingItemCreateRequest
            {
                Name = Guid.NewGuid().ToString(),
                Quantity = 10,
                Price = 20,
                ShoppingListId = listId,
            };

            _context.
                Setup(x => x.ShoppingLists
                .FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((ShoppingList)null);

            Func<Task> act = async () => await _service.CreateShoppingItem(createRequest);
            await act.Should().ThrowAsync<Exception>().WithMessage("A lista de compras especificada não existe.");
        }

        [Fact(DisplayName = "003 - Deve retornar falso se o item de compra não for encontrado")]
        public async Task RemoveShoppingItem_ShouldReturnFalse_WhenItemDoesNotExist() 
        {
            //testo se quando nulo meu metodo retorno falso
            _context
                .Setup(x => x.ShoppingItems
                .FindAsync(It.IsAny<long>()))
                .ReturnsAsync((ShoppingItem)null);//defino meu findasync p/ retornar nulo

            var result = await _service.RemoveShoppingItem(999);

            result.Should().BeFalse();
            _context.Verify(X => X.SaveChangesAsync(default), Times.Never);
        }

        [Fact(DisplayName = "004 - Deve retornar falso se a lista de compras associada não for encontrada")]
        public async Task RemoveShoppingItem_ShouldReturnFalse_WhenShoppingListDoesNotExist() 
        {
            //metodo passa 1° pelo findasync de item, então mocko o item para depois procurar a lista 
            var listId = 123;
            var itemId = 1;

            var shoppingItem = new ShoppingItem
            {
                Id = itemId,
                ShoppingListId = listId,
                Price = 10,
                Quantity = 2
            };
 
            _context //mocko o item existente
                .Setup(x => x.ShoppingItems
                .FindAsync(itemId))
                .ReturnsAsync(shoppingItem);

            _context
                .Setup(x => x.ShoppingLists
                .FindAsync(listId))
                .ReturnsAsync((ShoppingList)null);

            var result = await _service.RemoveShoppingItem(itemId);
            result.Should().BeFalse();
            _context.Verify(X => X.SaveChangesAsync(default), Times.Never);
        }

        [Fact(DisplayName = "005 - Deve retornar dados do item de compra pelo ID")]
        public async Task GetShoppingItemById_ShouldReturnItem_WhenItemExists() 
        {
            var itemId = 1;
            var listId = 123;
            var item = new ShoppingItem
            {
                Id = itemId,
                Name = Guid.NewGuid().ToString(),
                Quantity = 10,
                Price = 20,
                ShoppingListId = listId,
            };

            _context.Setup(x => x.ShoppingItems).ReturnsDbSet([item]);
            var result = await _service.GetShoppingItemById(itemId);
            result.Should().NotBeNull();
            result.Id.Should().Be(itemId);
            result.Name.Should().Be(item.Name);
            result.Quantity.Should().Be(item.Quantity);
            result.Price.Should().Be(item.Price);
        }

        [Fact(DisplayName = "006 - Deve retornar nulo se o item de compra não existir")]
        public async Task GetShoppingItemById_ShouldReturnNull_WhenItemDoesNotExist() 
        {
            var item = new ShoppingItem();
            var itemId = Random.Shared.Next();
            _context.Setup(x => x.ShoppingItems).ReturnsDbSet([item]);
            var result = await _service.GetShoppingItemById(itemId);
            result.Should().BeNull();
        }

        [Fact(DisplayName = "007 - Deve retornar lista paginada de itens de compra")]
        public async Task GetShoppingItem_ShouldReturnPaginatedList_WhenPageAndSizeProvided() 
        {
            var page = 1;
            var pageSize = 10;
            var items = new List<ShoppingItem>
            {
                new () { Id = 1, Name = "Arroz", Price = 10, Quantity = 1 },
                new () { Id = 2, Name = "Feijão", Price = 5, Quantity = 3 }
            };

            _context.Setup(x => x.ShoppingItems).ReturnsDbSet(items);
            var result = await _service.GetShoppingItem(page, pageSize);
            result.Should().NotBeNull();
            result.CurrentPage.Should().Be(page);
            result.PageSize.Should().Be(pageSize);
            result.TotalItems.Should().Be(items.Count);
        }

        [Fact(DisplayName = "008 - Deve retornar lista vazia se não houver itens na página solicitada")]
        public async Task GetShoppingItem_ShouldReturnEmptyList_WhenNoItemsInPage()
        {
            var page = 2;
            var pageSize = 10;

            var items = new List<ShoppingItem>
            {
                new () { Id = 1, Name = "Arroz", Price = 10, Quantity = 1 }
            };
            _context.Setup(x => x.ShoppingItems).ReturnsDbSet(items);

            var result = await _service.GetShoppingItem(page, pageSize);
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
        }
        [Fact(DisplayName = "009 - Deve retornar a quantidade total de itens de compra")]
        public async Task GetTotal_ShouldReturnTotalCount_WhenCalled() 
        {
            var items = new List<ShoppingItem>
            {
                new () { Id = 1, Name = "Arroz", Price = 10, Quantity = 1 },
                new () { Id = 2, Name = "Feijão", Price = 5, Quantity = 3 }
            };
            _context.Setup(x => x.ShoppingItems).ReturnsDbSet(items);
            var result = await _service.GetTotal();
            result.Should().Be(items.Count);
        }
    }
}
