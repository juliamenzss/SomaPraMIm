using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomaPraMim.Domain.Entities
{
    public class ShoppingItem : ModelBase
    {
        public long ShoppingListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ShoppingList? ShoppingLists { get; set; }
    }
}