using LuduStack.Domain.Models;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Messaging.Queries.Course
{
    public class GetPlansQuery : CourseQuery<List<StudyPlan>>
    {
        public Guid CourseId { get; }

        public GetPlansQuery(Guid courseId)
        {
            CourseId = courseId;
        }
    }
}