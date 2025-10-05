using System;
using System.Linq;
using PgConsoleCrud.Data;
using PgConsoleCrud.Models;
using System.Text.Json;

class Program
{
    static void Main()
    {
        using var context = new AppDbContext();

        while (true)
        {
            Console.WriteLine("\nCRUD Operations:");
            Console.WriteLine("1. Create Employee");
            Console.WriteLine("2. Read All Employees");
            Console.WriteLine("3. Update Employee");
            Console.WriteLine("4. Delete Employee");
            Console.WriteLine("5. SearchEmployees");
            Console.WriteLine("6. Exit");
            Console.WriteLine("7. Export to CSV");
            Console.WriteLine("8. Export to JSON");
            Console.WriteLine("9. Exit");
            Console.WriteLine("10. Import from CSV");
            Console.WriteLine("11. Import from JSON");
            Console.WriteLine("12. Exit");

            Console.Write("Choose an option: ");
            var input = Console.ReadLine();

            //switch (input)
            //{
            //    case "1":
            //        CreateEmployee(context);
            //        break;
            //    case "2":
            //        ReadEmployees(context);
            //        break;
            //    case "3":
            //        UpdateEmployee(context);
            //        break;
            //    case "4":
            //        DeleteEmployee(context);
            //        break;
            //    case "5":
            //        return;
            //    default:
            //        Console.WriteLine("Invalid option.");
            //        break;
            //}

            switch (input)
            {
                case "1": CreateEmployee(context); break;
                case "2": ReadEmployees(context); break;
                case "3": UpdateEmployee(context); break;
                case "4": DeleteEmployee(context); break;
                case "5": SearchEmployees(context); break;
                case "6": return;
                case "7": ExportToCsv(context); break;
                case "8": ExportToJson(context); break;
                case "9": return;
                case "10": ImportFromCsv(context); break;
                case "11": ImportFromJson(context); break;
                case "12": return;

                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    //static void CreateEmployee(AppDbContext context)
    //{
    //    Console.Write("Name: ");
    //    var name = Console.ReadLine();
    //    Console.Write("Age: ");
    //    var age = int.Parse(Console.ReadLine() ?? "0");
    //    Console.Write("Department: ");
    //    var department = Console.ReadLine();

    //    var employee = new Employee { Name = name, Age = age, Department = department };
    //    context.Employees.Add(employee);
    //    context.SaveChanges();
    //    Console.WriteLine("Employee created.");
    //}

    static void CreateEmployee(AppDbContext context)
    {
        Console.Write("Name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        Console.Write("Age: ");
        if (!int.TryParse(Console.ReadLine(), out int age) || age <= 0)
        {
            Console.WriteLine("Invalid age.");
            return;
        }

        Console.Write("Department: ");
        var department = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(department))
        {
            Console.WriteLine("Department cannot be empty.");
            return;
        }

        var employee = new Employee { Name = name, Age = age, Department = department };
        context.Employees.Add(employee);
        context.SaveChanges();
        Console.WriteLine("Employee created.");
    }


    static void ReadEmployees(AppDbContext context)
    {
        var employees = context.Employees.ToList();
        Console.WriteLine("\nEmployees:");
        foreach (var emp in employees)
        {
            Console.WriteLine($"ID: {emp.Id}, Name: {emp.Name}, Age: {emp.Age}, Department: {emp.Department}");
        }
    }

    static void UpdateEmployee(AppDbContext context)
    {
        Console.Write("Enter Employee ID to update: ");
        int id = int.Parse(Console.ReadLine() ?? "0");
        var emp = context.Employees.Find(id);
        if (emp == null)
        {
            Console.WriteLine("Employee not found.");
            return;
        }

        Console.Write($"New Name (current: {emp.Name}): ");
        emp.Name = Console.ReadLine();
        Console.Write($"New Age (current: {emp.Age}): ");
        emp.Age = int.Parse(Console.ReadLine() ?? "0");
        Console.Write($"New Department (current: {emp.Department}): ");
        emp.Department = Console.ReadLine();

        context.SaveChanges();
        Console.WriteLine("Employee updated.");
    }

    static void DeleteEmployee(AppDbContext context)
    {
        Console.Write("Enter Employee ID to delete: ");
        int id = int.Parse(Console.ReadLine() ?? "0");
        var emp = context.Employees.Find(id);
        if (emp == null)
        {
            Console.WriteLine("Employee not found.");
            return;
        }

        context.Employees.Remove(emp);
        context.SaveChanges();
        Console.WriteLine("Employee deleted.");
    }

    static void SearchEmployees(AppDbContext context)
    {
        Console.Write("Search by Name or Department: ");
        string keyword = Console.ReadLine()?.ToLower();

        var results = context.Employees
            .Where(e => e.Name.ToLower().Contains(keyword) || e.Department.ToLower().Contains(keyword))
            .ToList();

        if (results.Count == 0)
        {
            Console.WriteLine("No matching employees found.");
            return;
        }

        Console.WriteLine("\nSearch Results:");
        foreach (var emp in results)
        {
            Console.WriteLine($"ID: {emp.Id}, Name: {emp.Name}, Age: {emp.Age}, Department: {emp.Department}");
        }
    }

    static void ExportToCsv(AppDbContext context)
    {
        var employees = context.Employees.ToList();
        var filePath = "employees.csv";

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Id,Name,Age,Department");
            foreach (var emp in employees)
            {
                writer.WriteLine($"{emp.Id},{emp.Name},{emp.Age},{emp.Department}");
            }
        }

        Console.WriteLine($"Employees exported to {filePath}");
    }
    static void ExportToJson(AppDbContext context)
    {
        var employees = context.Employees.ToList();
        var filePath = "employees.json";

        var json = JsonSerializer.Serialize(employees, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);

        Console.WriteLine($"Employees exported to {filePath}");
    }

    static void ImportFromCsv(AppDbContext context)
    {
        string filePath = "employees.csv";

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File '{filePath}' not found.");
            return;
        }

        var lines = File.ReadAllLines(filePath).Skip(1); // Skip header

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length != 4)
            {
                Console.WriteLine($"Skipping invalid line: {line}");
                continue;
            }

            if (!int.TryParse(parts[2], out int age))
            {
                Console.WriteLine($"Invalid age value: {parts[2]}");
                continue;
            }

            var employee = new Employee
            {
                Name = parts[1].Trim(),
                Age = age,
                Department = parts[3].Trim()
            };

            context.Employees.Add(employee);
        }

        context.SaveChanges();
        Console.WriteLine("CSV import completed.");
    }
    static void ImportFromJson(AppDbContext context)
    {
        string filePath = "employeesEx.json";

        
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File '{filePath}' not found.");
            return;
        }

        var json = File.ReadAllText(filePath);
        List<Employee>? employees;

        try
        {
            employees = JsonSerializer.Deserialize<List<Employee>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading JSON: " + ex.Message);
            return;
        }

        if (employees == null || employees.Count == 0)
        {
            Console.WriteLine("No data found in JSON.");
            return;
        }

        context.Employees.AddRange(employees);
        context.SaveChanges();
        Console.WriteLine("JSON import completed.");
    }


}
