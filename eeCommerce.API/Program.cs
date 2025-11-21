using eCommerce.API.Middlewares;
using eCommerce.Core;
using eCommerce.Core.Mappers;
using eCommerce.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Add Infrastructure services
builder.Services.AddInfrastructure();
builder.Services.AddCore();

// Add Controllers
builder.Services.AddControllers().AddJsonOptions
    (options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Add AutoMapper   
builder.Services.AddAutoMapper(typeof(ApplicationUserMappingProfile).Assembly);

var app = builder.Build();

//Exception handler middleware

app.UseExceptionHandlingMiddleware();

//Routing

app.UseRouting();

//Auth

app.UseAuthorization();
app.UseAuthorization();

//Controller routes

app.MapControllers();


app.Run();
