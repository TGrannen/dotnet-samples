namespace EFCore.Web.Persistence;

public class DbInitializer(ILogger<DbInitializer> logger, SchoolContext context)
{
    public async Task MigrateAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to migrate database.");
            throw;
        }
    }

    public async Task SeedIfEmpty()
    {
        // Look for any students.
        if (context.Students.Any())
        {
            return; // DB has been seeded
        }

        try
        {
            AutoFaker.Configure(builder => { builder.WithConventions(); });
            Randomizer.Seed = new Random(8675309);

            var studentFaker = new AutoFaker<Student>()
                    .RuleFor(x => x.FirstMidName, f => f.Person.FirstName)
                    .RuleFor(x => x.LastName, f => f.Person.LastName)
                    .RuleFor(x => x.EnrollmentDate, f => f.Date.Past(100).ToUniversalTime())
                    .RuleFor(x => x.Enrollments, f => new List<Enrollment>())
                ;
            var students = studentFaker.Generate(10).ToArray();
            context.Students.AddRange(students);

            // TODO remove multiple saves
            await context.SaveChangesAsync();

            var courseFaker = new Faker<Course>()
                    .RuleFor(x => x.Title, f => f.Name.JobTitle())
                    .RuleFor(x => x.ClassroomId, f => null)
                    .RuleFor(x => x.Enrollments, f => new List<Enrollment>())
                ;
            var courses = courseFaker.Generate(10).ToArray();
            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            var enrollmentFaker = new Faker<Enrollment>()
                    .RuleFor(x => x.Student, f => f.PickRandom(students))
                    .RuleFor(x => x.Course, f => f.PickRandom(courses))
                    .RuleFor(x => x.Grade, f => f.Random.Int(1, 5) != 1 ? f.PickRandom<Grade>() : null)
                ;

            context.Enrollments.AddRange(enrollmentFaker.Generate(20));
            await context.SaveChangesAsync();

            var classroomFaker = new Faker<Classroom>()
                    .RuleFor(x => x.RoomNumber, f => f.Random.Int(14, 343).ToString())
                    .RuleFor(x => x.Courses, f =>
                    {
                        var skip = f.Random.Int(0, 3);
                        var take = f.Random.Int(1, 5);
                        return context.Courses.Skip(skip).Take(take).ToList();
                    })
                ;
            var classrooms = classroomFaker.Generate(10).ToArray();
            context.Classrooms.AddRange(classrooms);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to seed database with sample data.");
            throw;
        }
    }
}