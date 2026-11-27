using Microsoft.EntityFrameworkCore;
using Zoo.Common.Interfaces;
using Zoo.Common.Services;
using Zoo.Infrastructure;
using Zoo.Infrastructure.Models;
using Zoo.Infrastructure.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
// Configure Swagger/OpenAPI for API documentation
builder.Services.AddSwaggerGen();

// Configure controllers
builder.Services.AddControllers();

// Configure Entity Framework Core with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5432;Database=ZooDb;Username=postgres;Password=8gyrg8104ife";
builder.Services.AddDbContext<ZooContext>(options =>
    options.UseNpgsql(connectionString));

// Register repositories using Dependency Injection
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

// Register CRUD services
builder.Services.AddScoped<ICrudServiceAsync<AnimalModel>>(sp =>
    new CrudServiceAsync<AnimalModel>(sp.GetRequiredService<IRepository<AnimalModel>>()));

builder.Services.AddScoped<ICrudServiceAsync<EnclosureModel>>(sp =>
    new CrudServiceAsync<EnclosureModel>(sp.GetRequiredService<IRepository<EnclosureModel>>()));

builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

app.Run();

app.Run();
