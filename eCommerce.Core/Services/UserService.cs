using eCommerce.Core.DTO;
using eCommerce.Core.Entities;
using eCommerce.Core.RepositoryContracts;
using eCommerce.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Core.Services
{
    internal class UserService : IUserService
    {
        private readonly IUsersRepository _repository;

        public UserService(IUsersRepository repository)
        {
            this._repository = repository;
        }

        public async Task<AuthenticationResponse?> Login(LoginRequest loginRequest)
        {
            var user = await _repository.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password);
            if (user == null)
                return null;
            return new AuthenticationResponse(user.UserId,user.Email,user.PersonName,user.Gender,"Token",true);
        }

        public async Task<AuthenticationResponse?> Register(RegisterRequest registerRequest)
        {
            // Create a new Application user from the requests

            ApplicationUser applicationUser = new ApplicationUser()
            {
                 PersonName = registerRequest.PersonName,
                  Email= registerRequest.Email,
                  Password = registerRequest.Password,
                  Gender  =  registerRequest.Gender.ToString(),
            };

           ApplicationUser? registeredUser = await _repository.AddUser(applicationUser);
            if (registeredUser == null)
                return null;
            return new AuthenticationResponse(registeredUser.UserId, registeredUser.PersonName, registeredUser.Gender, registeredUser.Email, "token", true);



        }
    }
}
