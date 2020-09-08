using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Application.Commands
{
    public abstract class CourseCommand : Command
    {
        public Guid Id { get; internal set; }

        public CourseCommand(Guid id)
        {
            Id = id;
            AggregateId = id;
        }
    }
}
