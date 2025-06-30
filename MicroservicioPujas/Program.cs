using Infrastructure.Persistance;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Reflection;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Consumer;
using Infrastructure.Repositories.MongoDB;
using Infrastructure.Repositories.PostgreSQL;
using Application.Command;
using Application.External_Services.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mi API",
        Version = "v1",
        Description = "Documentación de mi API usando Swagger"
    });
});

// PostgreSQL
builder.Services.AddDbContext<SubastaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"),
        b => b.MigrationsAssembly("Infrastructure")));

// MongoDB
var mongoClient = new MongoClient("mongodb://localhost:27017");
builder.Services.AddSingleton<IMongoClient>(mongoClient);

// MediatR y HttpClients
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddHttpClient<UsuarioService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/api/usuarios/");
});

builder.Services.AddHttpClient<SubastaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5003/api/Subastas/");
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegistrarPujaCommand).Assembly));

// SignalR
builder.Services.AddSignalR();

// CORS ✅ Aquí va la configuración de CORS (fuera de RabbitMQ)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5000", "http://localhost:5173")  // Tu frontend React
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Repositorios y Servicios
builder.Services.AddScoped<IPujaMongoRepository, PujaMongoRepository>();
builder.Services.AddScoped<IPujaPostgreSQLRepository, PujaPostgreSQLRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ISubastaService, SubastaService>();
builder.Services.AddScoped<IPujaService, PujaService>();

// Configuración de RabbitMQ con MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PujaRegistradaConsumer>();
    x.AddConsumer<PujasAutomaticasConsumer>();
    x.AddConsumer<ActualizarMontoSubastaConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("puja-registrada-queue", e =>
        {
            e.ConfigureConsumer<PujaRegistradaConsumer>(context);
        });

        cfg.ReceiveEndpoint("pujaAutomatica-registrada-queue", e =>
        {
            e.ConfigureConsumer<ActualizarMontoSubastaConsumer>(context);
        });

                cfg.ReceiveEndpoint("monto-actualizado-queue", e =>
        {
            e.ConfigureConsumer<PujasAutomaticasConsumer>(context);
        });
        cfg.ReceiveEndpoint("monto-actualizado-queue", e =>
        {
            e.ConfigureConsumer<ActualizarMontoSubastaConsumer>(context);
        });
    });
});

var app = builder.Build();

app.UseRouting();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
        c.RoutePrefix = "swagger";
    });
}
app.UseCors();
app.UseAuthorization();

app.MapHub<PujasHub>("/Pujashub").RequireCors(policy =>
    policy.WithOrigins("http://localhost:5000", "http://localhost:5173")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()
);
app.MapControllers();

app.Run();
