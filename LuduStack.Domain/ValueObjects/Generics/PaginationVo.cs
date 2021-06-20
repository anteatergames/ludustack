using LuduStack.Domain.Interfaces.Models;

namespace LuduStack.Domain.ValueObjects
{
    public class PaginationVo : IPagination
    {
        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int TotalPageCount { get; set; }

        public string PaginationMessage { get; set; }

        public bool Bottom { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
    }
}