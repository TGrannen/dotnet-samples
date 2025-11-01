using System.ComponentModel.DataAnnotations;

namespace EFCore.Web.Models;

public class Student
{
    public int Id { get; set; }
    [MaxLength(400)] public required string LastName { get; set; }
    [MaxLength(400)] public required string FirstMidName { get; set; }
    public DateTime EnrollmentDate { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
}