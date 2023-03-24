using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public ActionResult Login(string username, string password)
    {
        if (username.Equals("admin") && password.Equals("admin"))
        {
            return new JsonResult(new {userName = username, token = _tokenService.CreateToken(username)});
        }

        return Unauthorized("Invalid Credentials");
    }
}