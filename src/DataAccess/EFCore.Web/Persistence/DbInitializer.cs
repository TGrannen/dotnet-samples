namespace EFCore.Web.Persistence;

public class DbInitializer(ILogger<DbInitializer> logger, SchoolContext context, DataGenerator seeder)
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
            var students = seeder.GenerateStudents(10);
            context.Students.AddRange(students);
            var courses = seeder.GenerateCourses(10);
            context.Courses.AddRange(courses);
            var enrollments = seeder.GenerateEnrollments(students, courses);
            context.Enrollments.AddRange(enrollments);
            var classrooms = seeder.GenerateClassroom(10);
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