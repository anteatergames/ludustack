using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public abstract class GenericPaginatedListVo<T>
    {
        protected List<T> Items { get; set; }

        public PaginationVo Pagination { get; set; }

        protected GenericPaginatedListVo()
        {
            Items = new List<T>();
            Pagination = new PaginationVo();
        }
    }
}