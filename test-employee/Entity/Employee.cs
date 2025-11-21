using Microsoft.Data.SqlClient;
using System.Data;
using test_employee.Data;

namespace test_employee.Entity
{
    public class Employee(string fullName, DateTime birthday, string gender)
    {
        public string FullName { get; set; } = fullName;

        public DateTime Birthday { get; set; } = birthday.Date;

        public string Gender { get; set; } = gender;

        public int Age { get; set; }

        public int CalculateAge()
        {
            Age = DateTime.Now.Year - birthday.Year;
            if (DateTime.Now < birthday.AddYears(Age))
                Age--;

            return Age;
        }

        public async Task SaveAsync()
        {
            using SqlConnection context = new SqlConnection(DbConnection.connectionString);
            context.Open();

            var query = "INSERT INTO Employee(FullName, Birthday, Gender) VALUES(@fullName, @birthday, @gender)";
            using SqlCommand cmd = new SqlCommand(query, context);

            cmd.Parameters.Add("@fullName", SqlDbType.VarChar).Value = FullName;
            cmd.Parameters.Add("@birthday", SqlDbType.Date).Value = Birthday;
            cmd.Parameters.Add("@gender", SqlDbType.VarChar).Value = Gender;

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task SaveRangeAsync(Employee[] employees)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(DbConnection.connectionString);
                connection.Open();

                DataTable table = new DataTable();
                table.Columns.Add("FullName", typeof(string));
                table.Columns.Add("Birthday", typeof(DateTime));
                table.Columns.Add("Gender", typeof(string));

                Random rnd = new Random();

                for (int i = 0; i < 100; i++)
                {
                    var idx = rnd.Next(0, 3);
                    var e = employees[idx];
                    table.Rows.Add(e.FullName, e.Birthday, e.Gender);
                }

                for (int i = 0; i < 999900; i++)
                {
                    var idx = rnd.Next(0, employees.Length);
                    var e = employees[idx];
                    table.Rows.Add(e.FullName, e.Birthday, e.Gender);
                }

                using SqlBulkCopy bulk = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "Employee"
                };

                await bulk.WriteToServerAsync(table);

                Console.WriteLine("Записи успешно созданы."); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При создании записей возникла ошибка: {ex.Message}");
            }
        }

        public override string ToString()
        {
            return @$"
Имя: {FullName}
Дата рождения: {Birthday:dd-MM-yyyy}
Пол: {Gender}
Возраст: {Age}";
        }
    }
}
