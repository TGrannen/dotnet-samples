namespace EFCore.Web.Persistence;

public static class DbInitializer
{
    public static void Initialize(SchoolContext context)
    {
        if (context.Database.IsSqlServer())
        {
            context.Database.Migrate();
        }

        // Look for any students.
        if (context.Students.Any())
        {
            return; // DB has been seeded
        }

        var students = new Student[]
        {
            new() { FirstMidName = "Carson", LastName = "Alexander", EnrollmentDate = DateTime.Parse("2005-09-01") },
            new() { FirstMidName = "Meredith", LastName = "Alonso", EnrollmentDate = DateTime.Parse("2002-09-01") },
            new() { FirstMidName = "Arturo", LastName = "Anand", EnrollmentDate = DateTime.Parse("2003-09-01") },
            new() { FirstMidName = "Gytis", LastName = "Barzdukas", EnrollmentDate = DateTime.Parse("2002-09-01") },
            new() { FirstMidName = "Yan", LastName = "Li", EnrollmentDate = DateTime.Parse("2002-09-01") },
            new() { FirstMidName = "Peggy", LastName = "Justice", EnrollmentDate = DateTime.Parse("2001-09-01") },
            new() { FirstMidName = "Laura", LastName = "Norman", EnrollmentDate = DateTime.Parse("2003-09-01") },
            new() { FirstMidName = "Nino", LastName = "Olivetto", EnrollmentDate = DateTime.Parse("2005-09-01") }
        };
        foreach (Student s in students)
        {
            context.Students.Add(s);
        }

        context.SaveChanges();

        var courses = new Course[]
        {
            new() { CourseId = 1050, Title = "Chemistry", Credits = 3 },
            new() { CourseId = 4022, Title = "Microeconomics", Credits = 3 },
            new() { CourseId = 4041, Title = "Macroeconomics", Credits = 3 },
            new() { CourseId = 1045, Title = "Calculus", Credits = 4 },
            new() { CourseId = 3141, Title = "Trigonometry", Credits = 4 },
            new() { CourseId = 2021, Title = "Composition", Credits = 3 },
            new() { CourseId = 2042, Title = "Literature", Credits = 4 }
        };
        foreach (Course c in courses)
        {
            context.Courses.Add(c);
        }

        context.SaveChanges();

        var enrollments = new Enrollment[]
        {
            new() { StudentId = 1, CourseId = 1050, Grade = Grade.A },
            new() { StudentId = 1, CourseId = 4022, Grade = Grade.C },
            new() { StudentId = 1, CourseId = 4041, Grade = Grade.B },
            new() { StudentId = 2, CourseId = 1045, Grade = Grade.B },
            new() { StudentId = 2, CourseId = 3141, Grade = Grade.F },
            new() { StudentId = 2, CourseId = 2021, Grade = Grade.F },
            new() { StudentId = 3, CourseId = 1050 },
            new() { StudentId = 4, CourseId = 1050 },
            new() { StudentId = 4, CourseId = 4022, Grade = Grade.F },
            new() { StudentId = 5, CourseId = 4041, Grade = Grade.C },
            new() { StudentId = 6, CourseId = 1045 },
            new() { StudentId = 7, CourseId = 3141, Grade = Grade.A },
        };
        foreach (Enrollment e in enrollments)
        {
            context.Enrollments.Add(e);
        }

        context.SaveChanges();

        var classrooms = new Classroom[]
        {
            new() { RoomNumber = "West 23", Courses = context.Courses.Take(1).ToList() },
            new() { RoomNumber = "North 4", Courses = context.Courses.Skip(2).Take(2).ToList() },
            new() { RoomNumber = "East 14", Courses = context.Courses.Skip(4).Take(1).ToList() },
        };

        context.Classrooms.AddRange(classrooms);
        context.SaveChanges();
    }
}