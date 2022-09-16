using dotnetcore6_rpg.Dtos.User;
using dotnetcore6_rpg.Service.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace dotnetcore6_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto userRegister)
        {
            try
            {
                var response = await _authService.Register
                    (new User { Username = userRegister.Username} , userRegister.Password);

                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Login")]

        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto userLogin)
        {
            try
            {
                var response = await _authService.Login(userLogin.Username, userLogin.Password);

                if (!response.Success) return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
