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
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
           _context = context;
        }

        public void Update(OrderHeader entity)
        {
           _context.OrderHeaders.Update(entity);
        }

        public void UpdateOrderStatus(int id, string ?OrderStatus, string? PaymentStatus=null)
        {
            var orderFromDb=_context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb != null) 
            {
                orderFromDb.PaymentDate = DateTime.Now;
                orderFromDb.OrederStatus = OrderStatus;
                if (PaymentStatus!=null)
                {
                    orderFromDb.PaymentStatus = PaymentStatus;
                }
            }
        }
    }
}
