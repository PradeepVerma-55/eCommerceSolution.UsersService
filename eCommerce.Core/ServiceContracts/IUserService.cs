using eCommerce.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Core.ServiceContracts
{
    public  interface IUserService
    {
        /// <summary>
        /// Method to handle user login use case.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        Task<AuthenticationResponse> Login(LoginRequest loginRequest);


        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <remarks>Ensure that all required fields in the <paramref name="registerRequest"/> object are
        /// populated before calling this method. The specific requirements for registration may vary depending on the
        /// implementation.</remarks>
        /// <param name="registerRequest">The registration details, including user credentials and other required information. This parameter cannot
        /// be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="AuthenticationResponse"/> object with details about the registration outcome, including any
        /// authentication tokens or status information.</returns>
        Task<AuthenticationResponse> Register(RegisterRequest registerRequest);
    }
}
