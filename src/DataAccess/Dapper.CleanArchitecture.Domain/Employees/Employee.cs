namespace Dapper.CleanArchitecture.Domain.Employees;

public class Employee
{
    public int EmpNo { get; set; }
    public DateTime BirthDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime HireDate { get; set; }
}