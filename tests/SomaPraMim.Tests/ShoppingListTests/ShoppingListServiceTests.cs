using FluentAssertions;
using SomaPraMim.Domain.Contexts;
using Moq;
using Moq.EntityFrameworkCore;
using SomaPraMim.Domain.Entities;
using SomaPraMim.Application.Services.ShoppingListServices;

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

        [Fact(DisplayName = "001 - Retorna valor total")]
        public async Task GivenShoppingListWhenGetTotalThenReturnsCorrectSum()
        {
            var shoppingListId = Random.Shared.Next(0, 100);

            var fakeShoppingList = new List<ShoppingItem>
            {
                new () {ShoppingListId = shoppingListId, Price = 15m, Quantity = 2},
                new () {ShoppingListId = shoppingListId, Price = 2.5m, Quantity = 4},
            };

            _context
                .Setup(x => x.ShoppingItems)
                .ReturnsDbSet(fakeShoppingList);

            var expectResult = await _service.GetShoppingListTotal(shoppingListId);

            expectResult.Should().Be(40m);
        }


        
    }
}