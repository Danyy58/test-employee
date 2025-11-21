using test_employee.Entity;

namespace test_employee.Service
{
    interface IEmployeeService
    {
        public Task CreateTableAsync();

        public Task CreateEmployeeAsync(string fullName, string birthdayString, string gender);

        public Task GetEmployeesAsync();

        public Task GetFMalesSlowAsync();

        public Task GetFMalesFastAsync();
    }
}
