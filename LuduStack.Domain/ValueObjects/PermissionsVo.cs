namespace LuduStack.Domain.ValueObjects
{
    public class PermissionsVo
    {
        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }

        public bool CanPostActivity { get; set; }

        public bool CanFollow { get; set; }

        public bool CanConnect { get; set; }

        public bool CanJoin { get; set; }

        public bool CanSubmit { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsMe { get; set; }

        public bool CanVote { get; set; }
    }
}