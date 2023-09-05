using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using UserApi.Models;
using UserApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    private string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt:Key").ToString() ?? "");
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Email, user.EmailConfirmed)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }

    [Route("login")]
    [HttpPost]
    public async Task<ActionResult> Login(UserLogin loginUser)
    {
        var user = await _userService.GetAsync(loginUser.EmailConfirmed, loginUser.PasswordHash);

        if (user is not null)
        {
            var token = GenerateToken(user);
            return Ok(new { token });
        }

        return BadRequest("User is not connected to an account");
    }

    [Route("register")]
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] UserRegister newUser)
    {
        var user = await _userService.GetAsync(newUser.Email, newUser.Password);

        if (user is null)
        {
            await _userService.CreateAsync(newUser);
            var _user = await _userService.GetAsync(newUser.Email, newUser.Password);
            var token = GenerateToken(_user!);
            return Ok(new { token });
        }

        return BadRequest("User account already exists");
    }

    [Authorize]
    [Route("auth")]
    [HttpGet]
    public ActionResult ValidateToken() => NoContent();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _userService.GetAsync(id);

        if (user is not null)
        {
            return user;
        }

        return NotFound();
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var user = await _userService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        updatedUser.Id = user.Id;
        await _userService.UpdateAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        await _userService.RemoveAsync(id);

        return NoContent();
    }
}