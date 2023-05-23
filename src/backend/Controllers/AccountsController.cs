using backend.Helpers;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [ValidateModel]
    public class AccountsController : Controller
    {
        private readonly IAccountServices _accountServices;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _userInContext;

        public AccountsController(IAccountServices accountServices, IHttpContextAccessor context)
        {
            _accountServices = accountServices;
            _contextAccessor = context;
            _userInContext = (User)_contextAccessor.HttpContext.Items["User"];            
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public async Task<IActionResult> Registration(RegistrationRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _accountServices.Register(request);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _accountServices.Login(request);

            return Ok(response);
        }
    }
}
