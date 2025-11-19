namespace eCommerce.Core.DTO
{
    public record AuthenticationResponse
    (
        Guid UserId,
        string? Email,
        string? PersonName,
        string? Gender,
        string? Token,
        bool success
    );
}
