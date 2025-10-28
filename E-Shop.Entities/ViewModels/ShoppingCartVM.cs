using E_Shop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Shop.Entities.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CartList { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public string Email {  get; set; }

    }
}
