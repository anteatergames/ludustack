namespace LuduStack.Domain.Interfaces.Models
{
    public interface IPaginatedQuery
    {
        public int Count { get; set; }

        public int Page { get; set; }
    }
}