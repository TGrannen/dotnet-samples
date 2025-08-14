namespace ObjectMapping.Facets.Models;

public class User
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public int Age { get; set; }
}

[Facet(typeof(User), nameof(User.Id), nameof(User.FirstName), nameof(User.LastName), Configuration = typeof(UserMapper))]
public partial class UserDto
{
    public string FullName { get; set; }
    public bool IsAdult => Age > 18;
}

public class UserMapper : IFacetMapConfiguration<User, UserDto>
{
    public static void Map(User source, UserDto target)
    {
        target.FullName = $"{source.FirstName} {source.LastName}";
    }
}

// Traditional mapping example
public class UserDto2
{
    public required string FullName { get; init; }
    public required string EmailAddress { get; init; }
    public required int Age { get; init; }
    public bool IsAdult => Age > 18;

    public static UserDto2 Map(User user) => new()
    {
        FullName = $"{user.FirstName} {user.LastName}",
        EmailAddress = user.EmailAddress ?? string.Empty,
        Age = user.Age
    };
}