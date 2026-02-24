using EmployeeAPI.Data;
using EmployeeAPI.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

//building services 
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetSection("AppDbContext")["ConnStr"] ?? throw new InvalidOperationException("Connection string not found.")));
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Program.cs mein add karein
builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
});

// Purani red line mita kar ye likhein
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeValidator>();

builder.Services.AddMemoryCache();
// 1. Service add karein
//builder.Services.AddResponseCaching();

var app = builder.Build();
app.UseResponseCompression();
//app.UseResponseCaching();

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
