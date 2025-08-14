namespace Mocking.AutoBogus;

public class Person
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    // public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public Address HomeAddress { get; set; }
    public List<string> Tags { get; set; }
    public Dictionary<string, int> Scores { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}