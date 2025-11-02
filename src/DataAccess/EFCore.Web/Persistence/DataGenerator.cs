namespace EFCore.Web.Persistence;

public class DataGenerator(SchoolContext context)
{
    private static readonly Faker<Course> _courseFaker = new Faker<Course>()
        .RuleFor(x => x.CourseId, f => 0)
        .RuleFor(x => x.Title, f => f.Name.JobTitle())
        .RuleFor(x => x.ClassroomId, f => null)
        .RuleFor(x => x.Enrollments, f => new List<Enrollment>());

    private static readonly Faker<Student> _studentFaker = new AutoFaker<Student>()
        .RuleFor(x => x.Id, f => 0)
        .RuleFor(x => x.FirstMidName, f => f.Person.FirstName)
        .RuleFor(x => x.LastName, f => f.Person.LastName)
        .RuleFor(x => x.EnrollmentDate, f => f.Date.Past(100).ToUniversalTime())
        .RuleFor(x => x.Enrollments, f => new List<Enrollment>());

    public IEnumerable<Student> GenerateStudents(int count)
    {
        return Enumerable.Range(0, int.MaxValue)
            .Select(_ => _studentFaker.Generate())
            .Take(count);
    }

    public IEnumerable<Course> GenerateCourses(int count)
    {
        return _courseFaker.Generate(count);
    }

    public Enrollment[] GenerateEnrollments(Student[] students, Course[] courses)
    {
        var enrollmentFaker = new Faker<Enrollment>()
                .RuleFor(x => x.EnrollmentId, f => 0)
                .RuleFor(x => x.Student, f => f.PickRandom(students))
                .RuleFor(x => x.Course, f => f.PickRandom(courses))
                .RuleFor(x => x.Grade, f => f.Random.Int(1, 5) != 1 ? f.PickRandom<Grade>() : null)
            ;

        return enrollmentFaker.Generate(20).ToArray();
    }

    public Classroom[] GenerateClassroom(int count)
    {
        var classroomFaker = new Faker<Classroom>()
                .RuleFor(x => x.Id, f => 0)
                .RuleFor(x => x.RoomNumber, f => f.Random.Int(14, 343).ToString())
                .RuleFor(x => x.Courses, f =>
                {
                    var skip = f.Random.Int(0, 3);
                    var take = f.Random.Int(1, 5);
                    return context.Courses.Skip(skip).Take(take).ToList();
                })
            ;
        return classroomFaker.Generate(count).ToArray();
    }
}