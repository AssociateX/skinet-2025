using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository,ProductRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

try
{
using var scope = app.Services.CreateScope(); // any code we create using scope variable then after they have been used the framework will dispose all the thing we used using scope
var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>(); //service locater pattern
    await context.Database.MigrateAsync(); //This line will apply any pending migrations (at this point the database will be created)
    await StoreContextSeed.SeedAsync(context); //Then this line will seed the data in the database
}

catch(Exception ex)
{
    Console.WriteLine(ex);
    throw;
}


app.Run();
