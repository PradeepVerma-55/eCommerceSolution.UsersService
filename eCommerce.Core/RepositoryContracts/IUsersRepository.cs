using eCommerce.Core.Entities;

namespace eCommerce.Core.RepositoryContracts
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Method to add a new user to the data store.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        Task<ApplicationUser?> AddUser(ApplicationUser user);

        /// <summary>
        /// Methods to get user by email and password.
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ApplicationUser> GetUserByEmailAndPassword(string? Email, string? password);
    }
}
