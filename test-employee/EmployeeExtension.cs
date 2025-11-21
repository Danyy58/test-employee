using System.Text;
using test_employee.Entity;

namespace test_employee
{
    public static class EmployeeExtension
    {
        public static string ToEmployeesString(this IEnumerable<Employee> employees)
        {
            var str = new StringBuilder();
            foreach (var e in employees)
            {
                str.AppendLine(e.ToString());
            }

            return str.ToString();
        }
    }
}
