using System.Collections.Generic;

namespace EFCore.Web.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public int? ClassroomId { get; set; }

        public string Title { get; set; }
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}