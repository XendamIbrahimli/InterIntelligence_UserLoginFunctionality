using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task1.Core.Dtos;
using Task1.Core.Services;

namespace Task1.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController(IAuthService _service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _service.Register(dto);
            if (!result)
            {
                return BadRequest("Failed to Register");
            }
            return Ok("Registered succesfully");

        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            return Ok(await _service.LoginAsync(dto));
        }
        [HttpGet]
        [Authorize]
        public IActionResult LogOut()
        {
            return Ok( _service.LogOut());
            //The server doesn't store the token, so there's nothing to delete. It processing on the frontend
        }


    }
}
