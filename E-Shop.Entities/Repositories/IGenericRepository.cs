﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Shop.Entities.Repositories
{
    public interface IGenericRepository<T>where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? predicate=null,string? IncludeWord = null);
        T GetFirstOrDefault(Expression<Func<T, bool>> ?predicate=null, string? IncludeWord = null);
        void Add (T entity);
        //void Update (T entity);
        void Remove (T entity);
        void RemoveRange(IEnumerable<T> entities);
    }

}
