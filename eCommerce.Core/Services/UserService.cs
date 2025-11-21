using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserService(IUsersRepository repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        public async Task<AuthenticationResponse?> Login(LoginRequest loginRequest)
        {
            var user = await _repository.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password);
            if (user == null)
                return null;
            return _mapper.Map<AuthenticationResponse>(user) with { Token="token", success = true};
        }

        public async Task<AuthenticationResponse?> Register(RegisterRequest registerRequest)
        {
            // Create a new Application user from the requests

            ApplicationUser applicationUser = _mapper.Map<ApplicationUser>(registerRequest);
            ApplicationUser? registeredUser = await _repository.AddUser(applicationUser);
            if (registeredUser == null)
                return null;
            return _mapper.Map<AuthenticationResponse>(registeredUser) with { Token = "token", success = true };



        }
    }
}
