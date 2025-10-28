using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Shop.Entities.Models;


namespace E_Shop.Entities.Repositories
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        void Update (OrderHeader entity);
        void UpdateOrderStatus (int id ,string OrderStatus,string PaymentStatus);
    }
}
