using Dapper;
using eCommerce.Core.DTO;
using eCommerce.Core.Entities;
using eCommerce.Core.RepositoryContracts;
using eCommerce.Infrastructure.DbContext;

namespace eCommerce.Infrastructure.Repositories
{
    internal class UsersRepository : IUsersRepository
    {
        private readonly DapperDbContext _dbContext;
        public UsersRepository(DapperDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ApplicationUser?> AddUser(ApplicationUser user)
        {
            // Generate a new UserId if not already provided
            if (user.UserId == Guid.Empty)
            {
                user.UserId = Guid.NewGuid();
            }
            const string query = @"
            INSERT INTO public.users (""UserId"", ""Email"", ""Password"", ""PersonName"", ""Gender"")
            VALUES (@UserId, @Email, @Password, @PersonName, @Gender); ";

            int rowAffected = await _dbContext.DbConnection.ExecuteAsync(query, user);
            if (rowAffected == 0)
               return null;
            return user;
        }

        public async Task<ApplicationUser?> GetUserByEmailAndPassword(string? email, string? password)
        {
            // Sql query to get user by email and password
            const string query = @"
            SELECT * FROM public.users
            WHERE ""Email"" = @Email AND ""Password"" = @Password; ";

            var user = await _dbContext.DbConnection.QuerySingleOrDefaultAsync<ApplicationUser>(query, new { Email = email, Password = password });
            return user;

        }
    }
}
