using eCommerce.Core.DTO;
using eCommerce.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]  // api/auth
    [ApiController]
    /// <summary>
    /// Controller to handle authentication operations such as user registration and login.
    /// </summary>
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        [Route("register")] // api/auth/register
        [HttpPost]
        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="registerRequest">The registration details. Cannot be null.</param>
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            //check if registerRequest is null
            if (registerRequest == null)
                return BadRequest("RegisterRequest cannot be null");
            // Call the service method to register the user
            var registrationResponse = await _userService.Register(registerRequest);
            if (registrationResponse == null || registrationResponse.success == false)
            {
                return BadRequest("User registration failed");
            }
            return Ok(registrationResponse);
        }

       /// <summary>
       /// Authenticates a user using provided login credentials.
       /// </summary>
       /// <param name="loginRequest">The login credentials. Cannot be null.</param>
       public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return BadRequest("LoginRequest cannot be null");
            var loginResponse = await _userService.Login(loginRequest);
            if (loginResponse == null || loginResponse.success == false)
            {
                return BadRequest("User login failed");
            }
            return Ok(loginResponse);
        }
    }
}
