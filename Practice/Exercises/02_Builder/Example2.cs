namespace Exercises.Builder.Example2;

// Builder - Vi du 2: Fluent Builder dung cau lenh SQL
// Xem lai: Builder-Pattern.md - Vi du 2
//
// Viet lai TU DAU cac thanh phan sau:
//
// - class SqlQuery (Product)
//     string Table { get; set; } = ""
//     List<string> Columns { get; } = new List<string>()
//     List<string> Conditions { get; } = new List<string>()
//     string ToSql() -> "SELECT {cot, hoac * neu rong} FROM {Table}", them " WHERE {dk1 AND dk2 ...}" neu co Conditions
// - class SqlQueryBuilder
//     Select(params string[] columns), From(string table), Where(string condition) -> return this
//     Build() -> neu Table rong thi throw InvalidOperationException("Table is required"), nguoc lai tra ve query

// Product
public class Employee
{
    public string Name { get; set; }
    public string Email { get; set; }
    public double Salary{ get; set; }
}

public class EmployeeBuilder
{
    private readonly Employee employee = new Employee();

    public EmployeeBuilder WithName(string name)
    {
        employee.Name = name;
        return this;
    }

    public EmployeeBuilder WithEmail(string email)
    {
        employee.Email = email;
        return this;
    }

    public EmployeeBuilder WithSalary(double salary, double coeff)
    {
        employee.Salary = salary * (coeff + 1);
        return this;
    }

    public Employee Build()
    {
        if(string.IsNullOrEmpty(employee.Name) || string.IsNullOrEmpty(employee.Email))
        {
            throw new InvalidOperationException("Name va Emal khong duoc bo trong");
        }
        return employee;
    }
}

// Cách dùng
public class Program
{
    public static void Main()
    {
        var employee = new EmployeeBuilder()
            .WithName("Nguyen Van An")
            .WithEmail("an@gmail.com")
            .WithSalary(20000000, 0.3)
            .Build();
    }
}