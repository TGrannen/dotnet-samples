using Dapper.Web.Services;

namespace Dapper.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeCrudController : ControllerBase
{
    private readonly IConnectionProvider _connectionProvider;

    public EmployeeCrudController(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var sql = @"
Select emp_no AS EmpNo,birth_date AS BirthDate,first_name AS FirstName,last_name AS LastName,gender AS gender,hire_date AS HireDate 
from employees";
        var employee = await _connectionProvider.Connection.QueryAsync<Employee>(sql);
        return Ok(employee);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> GetById(int employeeNumber)
    {
        var sql = @"
Select emp_no AS EmpNo,birth_date AS BirthDate,first_name AS FirstName,last_name AS LastName,gender AS gender,hire_date AS HireDate 
from employees
WHERE emp_no = @EmployeeNumber";
        var employee =
            await _connectionProvider.Connection.QueryFirstOrDefaultAsync<Employee>(sql, new { EmployeeNumber = employeeNumber });
        return Ok(employee);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(Employee employee)
    {
        var sql = @"
INSERT INTO employees (emp_no, birth_date, first_name, last_name, gender, hire_date)
VALUES(@EmpNo,@BirthDate,@FirstName,@LastName, 'M',@HireDate)
RETURNING emp_no";
        var id = await _connectionProvider.Connection.ExecuteScalarAsync<int>(sql, new
        {
            EmpNo = employee.EmpNo,
            BirthDate = employee.BirthDate,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            HireDate = employee.HireDate,
        });
        return Ok(id);
    }


    [HttpPatch("Update")]
    public async Task<IActionResult> Update(Employee employee)
    {
        var sql = @"
UPDATE employees SET 
     birth_date = @BirthDate,
     first_name = @FirstName,
     last_name = @LastName,
     hire_date = @HireDate
WHERE emp_no = @EmpNo
RETURNING emp_no";
        var saved = await _connectionProvider.Connection.ExecuteScalarAsync<int>(sql, new
        {
            EmpNo = employee.EmpNo,
            BirthDate = employee.BirthDate,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            HireDate = employee.HireDate,
        });
        return Ok(saved);
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> DeleteById(int employeeNumber)
    {
        var sql = @"
DELETE FROM employees
WHERE emp_no = @EmployeeNumber
RETURNING emp_no";
        var id = await _connectionProvider.Connection.ExecuteScalarAsync<int>(sql, new { EmployeeNumber = employeeNumber });
        return Ok(id);
    }

    public class Employee
    {
        public int EmpNo { get; set; }
        public DateTime BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime HireDate { get; set; }
    }
}