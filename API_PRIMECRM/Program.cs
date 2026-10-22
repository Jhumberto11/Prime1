using API_PRIMECRM.Application.Services.InventoryMovements;
using API_PRIMECRM.Application.Services.Master;
using API_PRIMECRM.Application.Services.Restock_Orders;
using API_PRIMECRM.Application.Services.Sales;
using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Interfaces.Inventory;
using API_PRIMECRM.Domain.Interfaces.Restock;
using API_PRIMECRM.Domain.Interfaces.Sales;
using API_PRIMECRM.Domain.Interfaces.Services;
using API_PRIMECRM.Infraestructure.Persistence;
using API_PRIMECRM.Infraestructure.Persistence.Repositories;
using API_PRIMECRM.Infraestructure.Persistence.Repositories.Inventory;
using API_PRIMECRM.Infraestructure.Persistence.Repositories.Restock_Order;
using API_PRIMECRM.Infraestructure.Persistence.Repositories.Sales;
using Microsoft.EntityFrameworkCore;
using PrimeCRM_Api.Infraestructure.Persistence;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


//Servicio de React
builder.Services.AddCors(options =>
{
    options.AddPolicy("React", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



// Repositorio Generico
builder.Services.AddScoped
    (
    typeof(IBaseRepository<>), typeof(BaseRepository<>)
    );

builder.Services.AddScoped<IRestockRepository, RestockOrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();




//// Servicios Especificos
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CourierCompanyAdminService>();
builder.Services.AddScoped<FreightCompanyAdminService>();
builder.Services.AddScoped<SaleChannelProvider>();
builder.Services.AddScoped<PaymentMethodProvider>();
builder.Services.AddScoped<IRestockOrderService, RestockOrderService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IInventoryMovementsService, InventoryMovementService>();




//Db Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




// ================================
// SWAGGER
// ================================
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

app.UseCors("React");

app.UseAuthorization();


app.MapControllers();

app.Run();
