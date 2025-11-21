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
    )
    {
        // Parameterless constructor for serialization/deserialization purposes
        public AuthenticationResponse() : this(default, default, default, default, default, false)
        {
        }
    }
}
