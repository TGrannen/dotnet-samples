namespace EFCore.Web.Models;

public class Classroom
{
    public int Id { get; set; }

    public string RoomNumber { get; set; }

    public ICollection<Course> Courses { get; set; }
}