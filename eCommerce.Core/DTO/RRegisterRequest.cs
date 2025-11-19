namespace eCommerce.Core.DTO
{
    public record RRegisterRequest(
        string? Email,
        string? Password,
        string? PersonName,
        GenderOptions Gender
        );
}