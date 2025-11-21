using Microsoft.Data.SqlClient;
using test_employee.Data;
using test_employee.Entity;

namespace test_employee.Repository
{
    class EmployeeRepository : IEmployeeRepository
    {
        public async Task CreateTableAsync()
        {
            var query = @"CREATE TABLE Employee (
                                  FullName VARCHAR(100) NOT NULL,
                                  Birthday DATE NOT NULL,
                                  Gender VARCHAR(1) NOT NULL);";

            await ExecuteQueryAsync(query);
        }

        public async Task<IEnumerable<Employee>> GetUniqueAsync()
        {
            var query = @"
                    SELECT  
                        FullName,
                        Birthday, 
                        Gender
                        FROM (
                            SELECT *,
                                   ROW_NUMBER() OVER(PARTITION BY FullName, Birthday ORDER BY FullName) AS rn
                            FROM Employee
                        ) AS t
                        WHERE rn = 1
                        ORDER BY FullName;";

            var employees = await GetEmployeesAsync(query);
            return employees;
        }

        public async Task<IEnumerable<Employee>> GetFMalesSlowAsync()
        {
            var query = @"
                        SELECT * FROM Employee
                        WHERE FullName LIKE 'F%'
                        AND Gender = 'M'";

            var employees = await GetEmployeesAsync(query);
            return employees;
        }

        public async Task<IEnumerable<Employee>> GetFMalesFastAsync()
        {
            var query = @"
                    SELECT * FROM Employee
                    WHERE Initial = 'F'
                    AND Gender = 'M'";

            var employees = await GetEmployeesAsync(query);
            return employees;
        }

        public async Task OptimizeOnAsync()
        {
            var query = @"
                    ALTER TABLE Employee
                    ADD Initial AS (SUBSTRING(FullName, 1, 1));

                    CREATE NONCLUSTERED INDEX idx_gender_initial_include
                    ON Employee(Gender, Initial)                    
                    INCLUDE (FullName, Birthday);

                    UPDATE STATISTICS Employee;";

            await ExecuteQueryAsync(query);
        }

        public async Task OptimizeOffAsync()
        {
            var query = @" 
                    DROP INDEX idx_gender_initial_include ON Employee;
                    ALTER TABLE Employee DROP COLUMN Initial;";

            await ExecuteQueryAsync(query);
        }

        
        private async Task ExecuteQueryAsync(string query)
        {
            using SqlConnection context = new SqlConnection(DbConnection.connectionString);
            context.Open();

            using SqlCommand cmd = new SqlCommand(query, context);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task<IEnumerable<Employee>> GetEmployeesAsync(string query)
        {
            using SqlConnection context = new SqlConnection(DbConnection.connectionString);
            context.Open();
            
            using SqlCommand cmd = new SqlCommand(query, context);
            using var reader = await cmd.ExecuteReaderAsync();

            List<Employee> employees = new List<Employee>();
            while (reader.Read())
            {
                var fullName = reader["FullName"].ToString()!;
                var birthday = Convert.ToDateTime(reader["Birthday"]);
                var gender = reader["Gender"].ToString()!;

                var employee = new Employee(fullName, birthday, gender);
                employee.CalculateAge();
                employees.Add(employee);
            }

            return employees;
        }
    }
}