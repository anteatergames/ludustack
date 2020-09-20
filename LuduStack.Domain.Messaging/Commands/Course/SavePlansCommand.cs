using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Messaging
{
    public class SavePlansCommand : CourseCommand
    {
        public Guid CourseId { get; }
        public List<StudyPlan> Plans { get; }

        public SavePlansCommand(Guid courseId) : base(courseId)
        {
        }

        public SavePlansCommand(Guid courseId, List<StudyPlan> plans) : base(courseId)
        {
            Plans = plans;
        }

        public override bool IsValid()
        {
            Result.Validation = new SavePlansCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }
}