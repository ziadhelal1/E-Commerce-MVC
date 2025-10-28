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
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailRepository(ApplicationDbContext context) : base(context)
        {
           _context = context;
        }


        public void Update(OrderDetail entity)
        {
            _context.OrderDetails.Update(entity);
        }
        
    }
}
