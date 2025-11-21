using test_employee.Data;
using test_employee.Entity;
using test_employee.Service;

namespace test_employee
{
    class EmployeeController(IEmployeeService employeeService)
    {
        public async Task ExecuteAsync(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: Не указаны параметры для выполнения команды.");
                return;
            }
        
            switch (args[0])
            {
                case "1":
                    await employeeService.CreateTableAsync();
                    break;

                case "2":
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Ошибка: Недостаточно аргументов для создания новой записи.");
                        break;
                    }
                    await employeeService.CreateEmployeeAsync(args[1], args[2], args[3]);
                    break;

                case "3":
                    await employeeService.GetEmployeesAsync();
                    break;

                case "4":
                    await Employee.SaveRangeAsync(EmployeesData.employees);
                    break;

                case "5":
                    await employeeService.GetFMalesSlowAsync();
                    break;

                case "6":
                    await employeeService.GetFMalesFastAsync();
                    break;

                default:
                    break;
            }
        }
    }
}
