using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Web.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Course");
        builder.Property(x => x.ClassroomId).IsRequired(false);
    }
}