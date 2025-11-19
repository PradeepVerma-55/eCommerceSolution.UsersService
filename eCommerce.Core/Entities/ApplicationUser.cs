namespace eCommerce.Core.Entities
{
    /// <summary>
    /// define the Application user class which acts as a data model for user information that will be stored in the system.
    /// </summary>
    public class ApplicationUser
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public  string? PersonName { get; set; }
        public string? Gender { get; set; }


    }
}
