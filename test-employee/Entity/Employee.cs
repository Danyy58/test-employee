using Microsoft.Data.Sqlite;
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
            using var context = new SqliteConnection(DbConnection.connectionString);
            context.Open();

            using var cmd = context.CreateCommand();
            
            var query = "INSERT INTO Employee(FullName, Birthday, Gender) VALUES(@fullName, @birthday, @gender)";
            cmd.CommandText = query;

            cmd.Parameters.Add("@fullName", SqliteType.Text).Value = FullName;
            cmd.Parameters.Add("@birthday", SqliteType.Text).Value = Birthday;
            cmd.Parameters.Add("@gender", SqliteType.Text).Value = Gender;

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task SaveRangeAsync(Employee[] employees)
        {
            try
            {
                using var connection = new SqliteConnection(DbConnection.connectionString);
                connection.Open();

                using var transaction = connection.BeginTransaction();

                using var command = connection.CreateCommand();
                command.CommandText =
                    "INSERT INTO Employee (FullName, Birthday, Gender) VALUES ($name, $birthday, $gender)";

                command.Parameters.Add("$name", SqliteType.Text);
                command.Parameters.Add("$birthday", SqliteType.Text);
                command.Parameters.Add("$gender", SqliteType.Text);

                var rnd = new Random();

                for (int i = 0; i < 100; i++)
                {
                    var e = employees[rnd.Next(0, 3)];

                    command.Parameters["$name"].Value = e.FullName;
                    command.Parameters["$birthday"].Value = e.Birthday;
                    command.Parameters["$gender"].Value = e.Gender;

                    command.ExecuteNonQuery();
                }

                for (int i = 0; i < 999900; i++)
                {
                    var e = employees[rnd.Next(0, employees.Length)];

                    command.Parameters["$name"].Value = e.FullName;
                    command.Parameters["$birthday"].Value = e.Birthday;
                    command.Parameters["$gender"].Value = e.Gender;

                    command.ExecuteNonQuery();
                }

                transaction.Commit();

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
