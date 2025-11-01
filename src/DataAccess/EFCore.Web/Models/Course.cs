using System.ComponentModel.DataAnnotations;

namespace EFCore.Web.Models;

public class Course
{
    public int CourseId { get; set; }
    public int? ClassroomId { get; set; }

    [MaxLength(400)] public required string Title { get; set; }
    public int Credits { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
}