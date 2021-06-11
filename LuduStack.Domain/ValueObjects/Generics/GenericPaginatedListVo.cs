using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.ValueObjects;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public abstract class GenericPaginatedListVo<T>
    {
        protected List<T> Items { get; set; }

        //public int TotalCount { get; set; }

        //public int Page { get; set; }

        //public int TotalPageCount { get; set; }

        //public string PaginationMessage { get; set; }

        public PaginationVo Pagination { get; set; }

        public GenericPaginatedListVo()
        {
            Items = new List<T>();
            Pagination = new PaginationVo();
        }
    }
}