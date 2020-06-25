using LuduStack.Domain.Core.Models;

namespace LuduStack.Domain.Models
{
    public class JobApplicant : Entity
    {
        public string CoverLetter { get; set; }

        public decimal Score { get; set; }

        public string Email { get; set; }
    }
}