namespace ObjectMapping.Facets.Models;

public class User
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public int Age { get; set; }
}

[Facet(typeof(User), nameof(User.Age), nameof(User.FirstName), nameof(User.LastName), Configuration = typeof(UserDtoMapper))]
public partial class UserDto
{
    public string FullName { get; set; }
    public bool IsAdult { get; set; }
}

public class UserDtoMapper : IFacetMapConfiguration<User, UserDto>
{
    public static void Map(User source, UserDto target)
    {
        target.FullName = $"{source.FirstName} {source.LastName}";
        target.IsAdult = source.Age > 18;
    }
}

// Traditional mapping example
public class UserDto2
{
    public required string FullName { get; init; }
    public required string EmailAddress { get; init; }
    public bool IsAdult { get; init; }

    public static UserDto2 Map(User user) => new()
    {
        FullName = $"{user.FirstName} {user.LastName}",
        EmailAddress = user.EmailAddress ?? string.Empty,
        IsAdult = user.Age > 18
    };
}