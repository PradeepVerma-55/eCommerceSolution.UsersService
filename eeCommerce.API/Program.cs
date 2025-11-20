using eCommerce.Infrastructure;
using eCommerce.Core;
using eCommerce.API.Middlewares;
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
