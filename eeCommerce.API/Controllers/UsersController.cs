using eCommerce.Core.DTO;
using eCommerce.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _usersService;

        public UsersController(IUserService usersService)
        {
            _usersService = usersService;
        }


        //GET /api/Users/{userID}
        [HttpGet("{userID}")]
        public async Task<IActionResult> GetUserByUserID(Guid userID)
        {
            await Task.Delay(100);
            throw new Exception("This is a exception for Poly retry testing purposes.");

            if (userID == Guid.Empty)
            {
                return BadRequest("Invalid User ID");
            }

            UserDTO? response = await _usersService.GetUserByUserID(userID);

            if (response == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
