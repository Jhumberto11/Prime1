using API_PRIMECRM.Application.Services.Master;
using API_PRIMECRM.Application.Services.Restock_Orders;
using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Interfaces.Restock;
using API_PRIMECRM.Domain.Interfaces.Services;
using API_PRIMECRM.Infraestructure.Persistence.Repositories;
using API_PRIMECRM.Infraestructure.Persistence.Repositories.Restock_Order;
using Microsoft.EntityFrameworkCore;
using PrimeCRM_Api.Infraestructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Repositorio Generico
builder.Services.AddScoped
    (
    typeof(IBaseRepository<>), typeof(BaseRepository<>)
    );

builder.Services.AddScoped<IRestockRepository, RestockOrderRepository>();

//// Servicios Especificos
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CourierCompanyAdminService>();
builder.Services.AddScoped<FreightCompanyAdminService>();
builder.Services.AddScoped<IRestockOrderService, RestockOrderService>();



//Db Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
