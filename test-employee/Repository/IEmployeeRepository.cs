using test_employee.Entity;

namespace test_employee.Repository
{
    interface IEmployeeRepository
    {
        Task CreateTableAsync();

        Task<IEnumerable<Employee>> GetUniqueAsync();

        Task<IEnumerable<Employee>> GetFMalesSlowAsync();

        Task<IEnumerable<Employee>> GetFMalesFastAsync();

        public Task OptimizeOnAsync();

        public Task OptimizeOffAsync();
    }
}
