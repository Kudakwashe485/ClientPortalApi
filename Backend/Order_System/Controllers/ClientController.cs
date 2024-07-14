using Microsoft.AspNetCore.Mvc;
using Order_System.models;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using Order_System.View_Models;
using Order_System.Helper;
using Order_System.models.Dto;
using Order_System.models.Login;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection;

namespace Order_System.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClientController : Controller
	{
			private readonly IConfiguration _configuration;
			private readonly AppDbContext _authContext;
			private readonly IRepository _repository;

			public ClientController(AppDbContext context, IRepository repository, IConfiguration configuration)
			{
				_authContext = context;
				_configuration = configuration;
				_repository = repository;
			}

		//**************************************************************************** Register Client *******************************************************************************
		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> AddClient([FromBody] ClientViewModel clientViewModel)
		{
			// Check if the clientViewModel is null
			if (clientViewModel == null)
				return BadRequest();

			// Check if email already exists
			if (await CheckEmailExistAsync(clientViewModel.Email))
				return BadRequest(new { Message = "Email already exists" });

			// Check if username already exists
			if (await CheckUsernameExistAsync(clientViewModel.Username))
				return BadRequest(new { Message = "Username already exists" });

			// Check password strength
			var passMessage = CheckPasswordStrength(clientViewModel.Password);
			if (!string.IsNullOrEmpty(passMessage))
				return BadRequest(new { Message = passMessage });

			// Create a new user object
			var newUser = new UserModel
			{
				FirstName = clientViewModel.FirstName,
				LastName = clientViewModel.LastName,
				Username = clientViewModel.Username,
				Password = PasswordHasher.HashPassword(clientViewModel.Password),
				Role = "User",
				Email = clientViewModel.Email,
				ContactNumber = clientViewModel.ContactNumber,
				PhysicalAddress = clientViewModel.PhysicalAddress,
				RefreshToken = "",
				RefreshTokenExpiryTime = DateTime.UtcNow,
				ResetPasswordToken = "",
				ResetPasswordTokenExpiry = DateTime.UtcNow,
			};

			// Add the new user to the database
			await _authContext.AddAsync(newUser);
			await _authContext.SaveChangesAsync();

			return Ok(new
			{
				Status = 200,
				Message = "You are registered successfully!"
			});
		}

		//**************************************************************************** User Login *******************************************************************************

		[HttpPost("Authenticate")]
		public async Task<IActionResult> Authenticate([FromBody] ClientViewModel clientViewModel)
		{
			// Check if the clientViewModel is null
			if (clientViewModel == null)
				return BadRequest();

			// Try to find the user in the database by their email
			var user = await _authContext.Users
				.FirstOrDefaultAsync(x => x.Email == clientViewModel.Email);

			// If the user is not found
			if (user == null)
				return NotFound(new { Message = "User not found!" });

			// Verify if the provided password matches the stored password
			if (!PasswordHasher.VerifyPassword(clientViewModel.Password, user.Password))
			{
				return BadRequest(new { Message = "Password is incorrect" });
			}

			// Create a new instance of JwtSecurityTokenHandler to handle the JWT token creation
			var tokenHandler = new JwtSecurityTokenHandler();
			// Get the secret key from configuration
			var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authentication");

			// Define the token descriptor with claims, expiration, and signing credentials
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
				  new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				  new Claim(ClaimTypes.Email, user.Email)
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),  // Set the signing credentials
				Issuer = _configuration["Tokens:Issuer"],
				Audience = _configuration["Tokens:Audience"]
			};

			// Create the token using the token handler and descriptor
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);
			user.Token = tokenString;
			// Generate a new refresh token
			var newRefreshToken = CreateRefreshToken();
			// Save the refresh token and its expiry time to the user object
			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(15);

			// Save the changes to the database
			await _authContext.SaveChangesAsync();

			return Ok(new TokenApiDto()
			{
				AccessToken = tokenString,
				RefreshToken = newRefreshToken
			});
		}

		//**************************************************************************** Email Validation *******************************************************************************
		private Task<bool> CheckEmailExistAsync(string? email)
				=> _authContext.Users.AnyAsync(x => x.Email == email);


		//**************************************************************************** Username Validation *******************************************************************************
		private Task<bool> CheckUsernameExistAsync(string? username)
				=> _authContext.Users.AnyAsync(x => x.Username == username);


		//**************************************************************************** Password Validation  *******************************************************************************
		private static string CheckPasswordStrength(string pass)
			{
				StringBuilder sb = new StringBuilder();
				if (pass.Length < 9)
					sb.Append("Minimum password length should be 8" + Environment.NewLine);
				if (!(Regex.IsMatch(pass, "[a-z]") && Regex.IsMatch(pass, "[A-Z]") && Regex.IsMatch(pass, "[0-9]")))
					sb.Append("Password should be AlphaNumeric" + Environment.NewLine);
				if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
					sb.Append("Password should contain special charcter" + Environment.NewLine);
				return sb.ToString();
			}


		//**************************************************************************** Create JWT token *******************************************************************************
		private string CreateJwt(UserModel user)
		{
			// Create a new instance of JwtSecurityTokenHandler to handle the JWT token creation
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			// Get the secret key from configuration
			var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authentication");

			// Define the token identity with claims, expiration, and signing credentials
			var identity = new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Role, user.Role),
				new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
				new Claim(ClaimTypes.Name,$"{user.Username}"),
				new Claim(ClaimTypes.GivenName,$"{user.FirstName}"),
				new Claim(ClaimTypes.Surname,$"{user.LastName}"),
				new Claim(ClaimTypes.WindowsAccountName,$"{user.ContactNumber}"),
				new Claim(ClaimTypes.Email,$"{user.Email}"),
				new Claim(ClaimTypes.Actor ,$"{user.PhysicalAddress }")

			});

			var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = identity,
				Expires = DateTime.Now.AddMinutes(15),
				SigningCredentials = credentials
			};
			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			return jwtTokenHandler.WriteToken(token);
		}


		//**************************************************************************** Create Refresh token *******************************************************************************
		private string CreateRefreshToken()
			{
				var tokenBytes = RandomNumberGenerator.GetBytes(64);
				var refreshToken = Convert.ToBase64String(tokenBytes);

				var tokenInUser = _authContext.Users
					.Any(a => a.RefreshToken == refreshToken);
				if (tokenInUser)
				{
					return CreateRefreshToken();
				}
				return refreshToken;
			}


			//**************************************************************************** Token Expiration *******************************************************************************
			private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
			{
				var key = Encoding.ASCII.GetBytes("veryverysceret.....");
				var tokenValidationParameters = new TokenValidationParameters
				{
					ValidateAudience = false,
					ValidateIssuer = false,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateLifetime = false
				};
				var tokenHandler = new JwtSecurityTokenHandler();
				SecurityToken securityToken;
				var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
				var jwtSecurityToken = securityToken as JwtSecurityToken;
				if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
					throw new SecurityTokenException("This is Invalid Token");
				return principal;

			}

			//**************************************************************************** Refresh *******************************************************************************
			[HttpPost]
			[Route("Refresh")]
			public async Task<IActionResult> Refresh([FromBody] TokenApiDto tokenApiDto)
			{
				if (tokenApiDto is null)
					return BadRequest("Invalid Client Request");
				string accessToken = tokenApiDto.AccessToken;
				string refreshToken = tokenApiDto.RefreshToken;
				var principal = GetPrincipleFromExpiredToken(accessToken);
				var username = principal.Identity.Name;
				var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Username == username);
				if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
					return BadRequest("Invalid Request");
				var newAccessToken = CreateJwt(user);
				var newRefreshToken = CreateRefreshToken();
				user.RefreshToken = newRefreshToken;
				await _authContext.SaveChangesAsync();
				return Ok(new TokenApiDto()
				{
					AccessToken = newAccessToken,
					RefreshToken = newRefreshToken,
				});
			}
		}
	}

