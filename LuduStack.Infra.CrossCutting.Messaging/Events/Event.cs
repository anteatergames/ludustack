using System;

namespace LuduStack.Infra.CrossCutting.Messaging
{
    public class Event
    {
        public DateTime Timestamp { get; private set; }

        protected Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}