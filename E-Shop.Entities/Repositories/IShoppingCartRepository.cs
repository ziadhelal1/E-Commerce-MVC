using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Shop.Entities.Models;


namespace E_Shop.Entities.Repositories
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        int IncreasementCount(ShoppingCart shoppingCart,int count);
        int DecreasementCount(ShoppingCart shoppingCart, int count);


    }
}
