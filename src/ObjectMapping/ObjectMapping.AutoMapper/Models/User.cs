namespace ObjectMapping.AutoMapper.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public int Age { get; set; }
    public bool IsAdult => Age > 18;
}