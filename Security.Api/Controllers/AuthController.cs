using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Security.Api.Controllers;

[Route("api/auth")]
public class AuthController : ControllerBase
{
	private readonly IConfiguration _configuration;

	public AuthController(IConfiguration configuration)
	{
		_configuration = configuration;

	}

	[HttpGet("/private")]
	[Authorize]
	public ActionResult<string> Private()
	{
		return Ok("Private");
	}

	[HttpPost("login")]
	public ActionResult<string> Login([FromBody] Credential credential)
	{
		if (credential.Username == "admin" && credential.Password == "password")
		{
			var claims = new List<Claim>
			{
				new(ClaimTypes.Name, "farhan"),
				new(ClaimTypes.Email,"farhan7534031b@gmail.com"),
			};

			var expireAt = DateTime.UtcNow.AddMinutes(8);

			var data = new
			{
				access_token = CreateToken(claims, expireAt),
				expireAt
			};

			return Ok(data);
		}

		ModelState.AddModelError("unauthenticated", "You are not authenticated");

		return Unauthorized(ModelState);
	}

	private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
	{
		var secretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("secretKey") ?? throw new Exception("secretKey is not defined"));

		var jwt = new JwtSecurityToken(
			claims: claims,
			notBefore: DateTime.UtcNow,
			expires: expireAt,
			signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
		);

		return new JwtSecurityTokenHandler().WriteToken(jwt);
	}
}

public class Credential
{
	[DefaultValue("admin")]
	public required string Username { get; set; }
	[DefaultValue("password")]
	public required string Password { get; set; }

}