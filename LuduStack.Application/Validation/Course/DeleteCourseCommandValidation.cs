using LuduStack.Application.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Application.Validation
{
    public class DeleteCourseCommandValidation : CourseValidation<DeleteCourseCommand>
    {
        public DeleteCourseCommandValidation()
        {
            ValidateId();
        }
    }
}
