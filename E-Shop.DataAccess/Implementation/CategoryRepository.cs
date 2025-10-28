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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
           _context = context;
        }
        public void Update(Category category)
        {
            var categryDb= _context.Categories.FirstOrDefault(x=>x.Id==category.Id);
            if (categryDb!=null)
            {
                categryDb.Name = category.Name;
                categryDb.Description = category.Description;
                categryDb.CreatedDate = DateTime.Now;

                // _context.Categories.Update(category);
            }

        }  
    }
}
