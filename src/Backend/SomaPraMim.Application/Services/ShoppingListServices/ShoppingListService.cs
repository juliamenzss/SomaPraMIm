using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SomaPraMim.Domain.Contexts;

namespace SomaPraMim.Application.Services.ShoppingListServices
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IShoppingListContext _context;

        public ShoppingListService(IShoppingListContext context)
        {
            _context = context;
        }


    }
}