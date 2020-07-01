using System;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects.Study
{
    public class StudyCoursesOfUserVo
    {
        public Guid UserId { get; set; }

        public List<UserCourseVo> Courses { get; set; }
    }

    public class UserCourseVo
    {
        public Guid CourseId { get; set; }

        public string CourseName { get; set; }
    }
}