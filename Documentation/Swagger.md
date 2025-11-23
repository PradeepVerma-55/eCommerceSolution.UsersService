Swagger (Swashbuckle) — Step‑by‑step enablement for this .NET 8 project

This guide shows how to enable Swagger (OpenAPI) using Swashbuckle in the API project (`eeCommerce.API`). Follow the steps below.

1) Install the NuGet package

- From project root or within `eeCommerce.API` directory run:

    dotnet add eeCommerce.API package Swashbuckle.AspNetCore

2) Enable XML documentation generation (optional but recommended)

- Edit `eeCommerce.API.csproj` and add the following inside the main `<PropertyGroup>` so generated XML comments appear in Swagger UI:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn> <!-- optional: suppress missing XML comment warnings -->
</PropertyGroup>
```

- Rebuild the project so the XML file is produced.

3) Register Swagger services in `Program.cs`

- In `eeCommerce.API\Program.cs` add (already present in this solution) the registration for Swagger generator. Example configuration with XML comments and basic metadata:

```csharp
using System.Reflection;

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "eCommerce Users API", Version = "v1" });

    // Include XML comments (if XML doc file is generated)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Optional: configure Bearer token input for JWT protected endpoints
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
```

4) Enable Swagger middleware in the HTTP pipeline (`Program.cs`)

- Add the middleware after `app.UseRouting()` and before `app.UseAuthorization()` (or conditionally in development). Example:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "eCommerce Users API v1");
        c.RoutePrefix = string.Empty; // serve Swagger UI at app root
    });
}
```

- If you want Swagger in non-development, remove the environment check.

5) Build and run

- Restore and build:

    dotnet restore
    dotnet build

- Run the API and open the Swagger UI (default):

    https://localhost:5001/ (or the URL shown in console)

6) Additional notes and best practices

- Secure Swagger UI in production. Do not expose sensitive endpoints or let anonymous users retrieve tokens through the UI without proper controls.
- Use XML comments on controllers/actions and DTOs to improve generated documentation.
- For minimal APIs or grouped endpoints you can use `c.SwaggerDoc` multiple times for versioning.
- If your app runs behind a reverse proxy or has a different base path, set `c.RoutePrefix` or `SwaggerEndpoint` appropriately.

7) Troubleshooting

- If XML comments are not showing, ensure `GenerateDocumentationFile` is enabled and the XML filename matches the assembly name used in `IncludeXmlComments`.
- If Swagger UI returns 404 for `swagger.json`, confirm `app.UseSwagger()` is called and the request path matches `SwaggerEndpoint`.

