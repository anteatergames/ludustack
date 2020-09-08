using LuduStack.Application.Validation;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Application.Commands
{
    public class DeleteCourseCommand : CourseCommand
    {
        public DeleteCourseCommand(Guid id) : base(id)
        {
        }

        public override bool IsValid()
        {
            ValidationResult = new DeleteCourseCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
