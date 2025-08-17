using Microsoft.EntityFrameworkCore;
using TrustFund.Domain.Repositories;
using TrustFund.Infrastructure.Context;
using TrustFund.Infrastructure.Core;
using TrustFund.Infrastructure.Repositories;
using TrustFund.Services.Contracts;
using TrustFund.Services.Mappers;
using TrustFund.Services.Services;
using System.Text.Json.Serialization; // Agrega esta línea

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configura el DbContext para la base de datos
builder.Services.AddDbContext<TrustFundDbContext>(options =>
    options.UseInMemoryDatabase("TrustFundDb")); // Opcional: Para pruebas, puedes usar una base de datos en memoria. Cambia esto a un SQL Server real en producción.

// Inyección de dependencias de la capa de repositorios
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Inyección de dependencias de la capa de servicios
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Configuración de AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers()
    .AddJsonOptions(options => // Para el enum LoanStatus
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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