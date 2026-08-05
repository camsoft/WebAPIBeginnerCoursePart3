using Microsoft.EntityFrameworkCore;
using WebAPICourse.Data;
using WebAPICourse.Repositories;
using WebAPICourse.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the EF Core DbContext, pointing it at SQL Server LocalDB using the
// "DefaultConnection" connection string from appsettings.json.
// AddDbContext registers AppDbContext as a "Scoped" service by default, meaning
// a new instance is created for each incoming HTTP request.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Program.cs
// NOTE: The repository is now Scoped (not Singleton) because it depends on AppDbContext,
// which is itself Scoped. A Singleton service cannot safely depend on a Scoped service.
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


var app = builder.Build();

// Automatically apply any pending EF Core migrations at startup.
// This is convenient for a learning project so students don't have to remember to run
// "dotnet ef database update" manually - the database and tables are created/updated
// the first time the app runs. In a production application you would typically apply
// migrations as part of your deployment pipeline instead of on every app startup.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
