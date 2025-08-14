namespace ObjectMapping.Mapster.Models;

public class User
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public int Age { get; set; }
}

public class UserDto
{
    public required string FullName { get; init; }
    public required string EmailAddress { get; init; }
    public required int Age { get; init; }
    public bool IsAdult => Age > 18;
}