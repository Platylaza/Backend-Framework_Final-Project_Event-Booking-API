using EventBookingApi.Services;
using EventBookingApi.Middleware;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Caching
builder.Services.AddMemoryCache();

// Rate Limiting

// Auth

// Custom Services
builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IBookingService, BookingService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyFirstApi",
        Version = "v1",
        Description = "An ASP.NET Core Web API for managing products",
        Contact = new OpenApiContact
        {
            Name = "API Support Team",
            Email = "support@example.com"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
