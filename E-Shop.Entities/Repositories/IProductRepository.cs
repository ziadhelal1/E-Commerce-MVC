using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Shop.Entities.Models;
using E_Shop.Entities.Repositories;

namespace E_Shop.Entities.Repositories
{
    public interface IProductRepository: IGenericRepository<Product>
    {
        void Update(Product entity);
    }
}
