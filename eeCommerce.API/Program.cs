using eCommerce.API.Middlewares;
using eCommerce.Core;
using eCommerce.Core.Mappers;
using eCommerce.Infrastructure;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Register infrastructure and core services (repositories, DB context, application services, etc.)
builder.Services.AddInfrastructure();
builder.Services.AddCore();

// Add MVC controllers and configure JSON options (serialize enums as strings)
builder.Services.AddControllers().AddJsonOptions
    (options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register AutoMapper profiles used to map between DTOs and domain entities
builder.Services.AddAutoMapper(typeof(ApplicationUserMappingProfile).Assembly);

// Enable FluentValidation automatic model validation for controller actions
builder.Services.AddFluentValidationAutoValidation();

// Register services for API metadata/explorer (used by Swagger)
builder.Services.AddEndpointsApiExplorer();

// Register Swagger generator to produce OpenAPI specification
builder.Services.AddSwaggerGen();

// Configure Cross-Origin Resource Sharing (CORS) policies
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        // Allow all origins for development/testing. Consider restricting this in production.
        builder.WithOrigins("*")
        .AllowAnyHeader()
        .AllowAnyMethod();

    });
});

var app = builder.Build();

// Global exception handling middleware to standardize error responses and logging
app.UseExceptionHandlingMiddleware();

// Use routing middleware to match incoming requests to endpoints
app.UseRouting();

// Enable middleware to serve generated Swagger as JSON endpoint
app.UseSwagger(); // Adds endpoints that can serve the swagger.json

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
app.UseSwaggerUI(); // Add Swagger UI (interactive page to explore and test API endpoints)

// Enable CORS using the configured policy
app.UseCors();

// Authentication/Authorization middleware
// Note: Ensure authentication middleware (e.g., app.UseAuthentication()) is added if authentication is required
app.UseAuthorization();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "eCommerce Users API v1");
        c.RoutePrefix = string.Empty; // serve Swagger UI at app root
    });
}

// Map controller routes
app.MapControllers();

// Start the application
app.Run();
