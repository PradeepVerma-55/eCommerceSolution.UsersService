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
            // Generate a new UserId    
            user.UserId = Guid.NewGuid();
            const string query = @"
            INSERT INTO users (userid, email, password, personname, gender)
            VALUES (@UserId, @Email, @Password, @PersonName, @Gender); ";

            int rowAffected = await _dbContext.DbConnection.ExecuteAsync(query, user);
            if (rowAffected == 0)
               return null;
            return user;
        }

        public async Task<ApplicationUser> GetUserByEmailAndPassword(string? email, string? password)
        {
            return new ApplicationUser
            {
                UserId = Guid.NewGuid(),
                Email = email,
                Password = password,
                PersonName = "Person Name",
                Gender = GenderOptions.Male.ToString()
            };
        }
    }
}
