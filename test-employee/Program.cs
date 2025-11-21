using Microsoft.Extensions.DependencyInjection;
using test_employee;
using test_employee.Repository;
using test_employee.Service;

var serviceProvider = new ServiceCollection()
    .AddSingleton<EmployeeController>()
    .AddScoped<IEmployeeService, EmployeeService>()
    .AddScoped<IEmployeeRepository, EmployeeRepository>()
    .BuildServiceProvider();

var controller = serviceProvider.GetRequiredService<EmployeeController>();
await controller.ExecuteAsync(args);