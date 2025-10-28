using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Shop.DataAccess.Data;
using E_Shop.Entities.Models;
using E_Shop.Entities.Repositories;


namespace E_Shop.DataAccess.Implementation
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
           _context = context;
        }

        public int DecreasementCount(ShoppingCart shoppingCart, int count)
        {
            if (shoppingCart.Count>1)
            {
                shoppingCart.Count -= count;
                return shoppingCart.Count;
            }
            return 1;
            
            
        }

        public int IncreasementCount(ShoppingCart shoppingCart, int count)
        {
            if (shoppingCart.Count < 100) 
            {
                shoppingCart.Count += count;
                return shoppingCart.Count;
            }
            return 100;
            
        }
    }
}
