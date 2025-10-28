using E_Shop.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Shop.Entities.ViewModels
{
    
    public class OrderVM
    {
        public OrderHeader  OrderHeader{ get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
