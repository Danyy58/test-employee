namespace test_employee.Data
{
    public static class DbConnection
    {
        public static readonly string connectionString = @"
            Server=localhost;
            Database=EmployeeDb;
            Trusted_Connection=True;
            TrustServerCertificate=True";
    }
}
