using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.Interfaces.Models
{
    public interface IPagination
    {
        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int TotalPageCount { get; set; }

        public string PaginationMessage { get; set; }
    }
}