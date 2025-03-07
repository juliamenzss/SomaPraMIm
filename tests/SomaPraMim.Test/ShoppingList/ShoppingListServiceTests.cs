using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;


namespace tests.SomaPraMim.Test.ShoppingList
{
    public class ShoppingListServiceTests
    {
        private readonly Mock<IShoppingListContext> _context;
        private readonly UserService _service;

        public ShoppingListServiceTests(){
            _context = new Mock<IShoppingListContext>();
            _service = new UserService(_context.Object);
        }

        [Fact(DisplayName = "001 - Retorna valor total")]
        public async GivenShoppingList_WhenGetTotal_ThenReturnsCorrectSum()
        {
            var shoppingListId = Random.Shared.Next(0, 100);

            var fakeShoppingLIst = new List<ShoppingList>{
                new () {ShoppingListId = shoppingListId, Price = 15m, Quantity = 2},
                new () {ShoppingListId = shoppingListId, Price = 2.5m, Quantity = 4},
            };

            _context
                .Setup(x => x.ShoppingItems)
                .ReturnDbSet(fakeShoppingLIst);

            var expectResult = await _service.GetShoppingListTotal(shoppingListId);

            expectResult.Should().Be(40m);
        }
    }
}