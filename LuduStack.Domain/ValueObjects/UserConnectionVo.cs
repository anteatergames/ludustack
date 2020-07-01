using LuduStack.Domain.Core.Enums;

namespace LuduStack.Domain.ValueObjects
{
    public class UserConnectionVo
    {
        public bool Accepted { get; set; }

        public UserConnectionDirection? Direction { get; set; }

        public UserConnectionType ConnectionType { get; set; }
    }
}