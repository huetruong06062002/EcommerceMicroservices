






using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Starting Customer API up");

try
{


    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");

    builder.Services.AddDbContext<CustomerContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
        .AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBaseAsync<,,>))
        .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
        .AddScoped<ICustomerService, CustomerService>();

    var app = builder.Build();
    app.MapGet("/",() => "Hello");

    app.MapGet("/api/customers",
        async (ICustomerService customerService) => await customerService.GetCustomersAsync());

    app.MapGet("/api/customers/{username}",
        async (string username, ICustomerService customerService) =>
        await customerService.GetCustomerByUsernameAsync(username));
    //app.MapPost("/", () => "Welcome to Customer API!");
    //app.MapPut("/", () => "Welcome to Customer API!");
    //app.MapDelete("/", () => "Welcome to Customer API!");




    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.SeedCustomerData()
        .Run();

}
catch (Exception ex)
{
    string type = ex.GetType().Name;

    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;
 
    Log.Fatal(ex, $"Unhandled exeption: {ex.Message}");
}
finally
{
    Log.Information("Shut down Customer API complete");
    Log.CloseAndFlush();
}