FluentValidation — Quick Integration & Help

This document explains how to integrate and use FluentValidation in this project (ASP.NET Core + .NET 8).

1) Install package

Run in the project that hosts your API (e.g. `eeCommerce.API`):

    dotnet add eeCommerce.API package FluentValidation.AspNetCore

2) Add validators

Create validator classes that implement `AbstractValidator<T>` (examples already exist in `eCommerce.Core\Validators`). Example:

```csharp
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
```

3) Register FluentValidation in `Program.cs`

In `eeCommerce.API\Program.cs` register FluentValidation and scan for validators:

```csharp
// using FluentValidation;
// using FluentValidation.AspNetCore;

builder.Services.AddControllers();
// automatic model validation via FluentValidation
builder.Services.AddFluentValidationAutoValidation();
// register validators from assemblies (adjust type used to point to an assembly containing your validators)
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
```

Notes:
- `AddFluentValidationAutoValidation()` enables automatic validation of controller action parameters (model binding) using FluentValidation.
- `AddValidatorsFromAssemblyContaining<T>()` scans the assembly containing `T` and registers all `IValidator<>` implementations.

4) Returning validation errors to clients

ASP.NET Core by default returns a `400 Bad Request` when model validation fails. If you want a custom response format, configure `ApiBehaviorOptions`:

```csharp
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(kvp => kvp.Value.Errors.Count > 0)
            .Select(kvp => new
            {
                Field = kvp.Key,
                Messages = kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            });

        return Results.BadRequest(new { Errors = errors });
    };
});
```

5) Manual validation (service layer)

You can inject `IValidator<T>` or `IValidatorFactory` and validate manually when needed:

```csharp
public class MyService
{
    private readonly IValidator<LoginRequest> _validator;
    public MyService(IValidator<LoginRequest> validator) => _validator = validator;

    public async Task Validate(LoginRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            // map/throw or return errors
            throw new ValidationException(result.Errors);
        }
    }
}
```

6) Common pitfalls & tips

- PostgreSQL / DB errors are unrelated to FluentValidation; keep validators focused on input validation only.
- Ensure validators are in an assembly that gets scanned by `AddValidatorsFromAssemblyContaining<...>()`.
- If validation doesn't run for controller parameters, ensure `AddFluentValidationAutoValidation()` is present and FluentValidation package is referenced in the API project.
- For minimal APIs you might need to validate explicitly or use the `FluentValidation.AspNetCore` integration for minimal APIs.

7) Useful references

- Official docs: https://docs.fluentvalidation.net/
- Package: `FluentValidation.AspNetCore` on NuGet

If you want, I can add the required registration lines into `eeCommerce.API\Program.cs` for you and wire up a custom `InvalidModelStateResponseFactory` to standardize error responses.