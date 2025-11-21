using System.Diagnostics;
using System.Globalization;
using test_employee.Entity;
using test_employee.Repository;

namespace test_employee.Service
{
    class EmployeeService(IEmployeeRepository repo) : IEmployeeService
    {
        public async Task CreateTableAsync()
        {
            try
            {
                await repo.CreateTableAsync();
                Console.WriteLine("Таблица Employee была успешно создана.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При создании таблицы возникла ошибка: {ex.Message}.");
            }
        }

        public async Task CreateEmployeeAsync(string fullName, string birthdayString, string gender)
        {                
            try 
            {
                DateTime birthday = DateTime.ParseExact(birthdayString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var employee = new Employee(fullName, birthday, gender);
                await employee.SaveAsync();
                Console.WriteLine($"Сотрудник {fullName}, {birthday}, {gender} был успешно добавлен в таблицу.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При добавлении сотрудника возникла ошибка: {ex.Message}.");
            }
        }

        public async Task GetEmployeesAsync()
        {
            try
            {
                IEnumerable<Employee> employees = await repo.GetUniqueAsync();
                Console.WriteLine(employees.ToEmployeesString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При получении сотрудников возникла ошибка: {ex.Message}.");
            }
        }

        public async Task GetFMalesSlowAsync()
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var employees = await repo.GetFMalesSlowAsync();
                sw.Stop();

                LogTimedQueryResult(employees, sw);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При выполнении неоптимизированного запроса возникла ошибка: {ex.Message}");
            }
        }

        public async Task GetFMalesFastAsync()
        {
            try
            {
                await repo.OptimizeOnAsync();

                Stopwatch sw = Stopwatch.StartNew();
                var employees = await repo.GetFMalesFastAsync();
                sw.Stop();

                LogTimedQueryResult(employees, sw);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При выполнении оптимизированного запроса возникла ошибка: {ex.Message}");
            }
            finally
            {
                await repo.OptimizeOffAsync();
            }
        }


        private void LogTimedQueryResult(IEnumerable<Employee> employees, Stopwatch sw)
        {
            Console.WriteLine(employees.ToEmployeesString());
            Console.WriteLine($"Время: {sw.ElapsedMilliseconds} ms");
        }
    }
}
