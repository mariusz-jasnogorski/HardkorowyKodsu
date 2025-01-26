using HardkorowyKodsu.Server.Data;
using HardkorowyKodsu.Server.Middlewares;
using HardkorowyKodsu.Server.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HardkorowyKodsuDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IDatabaseSchemaRepository, DatabaseSchemaRepository>();
builder.Services.AddScoped<IDatabaseSchemaService, DatabaseSchemaService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseGlobalExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
