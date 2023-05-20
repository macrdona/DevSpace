using backend.Helpers;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : Controller
    {
        private readonly IAccountServices _accountServices;
        public AccountsController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Registration(RegistrationRequest request)
        {
            await _accountServices.Register(request);

            return Ok("User has registered succesfully");
        }
    }
}
