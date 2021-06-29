using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using XiechengAPI.Dtos;
using XiechengAPI.Moldes;

namespace XiechengAPI.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticateController(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginDto loginDto)
        {
            // validate username and password
            var loginResult = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false,
                false
            );

            if (!loginResult.Succeeded)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(loginDto.Email);
            // create JWT if success
            // header
            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            // payload
            var claims = new List<Claim>()
            {
                // sub
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                // new Claim(ClaimTypes.Role,"Admin")
            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);

            }
            // signature
            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecurityKey"]);
            var signingKey = new SymmetricSecurityKey(secretByte);
            var singingCredentials = new SigningCredentials(signingKey, signingAlgorithm);
            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires:DateTime.UtcNow.AddDays(2),
                singingCredentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            // return 200 ok + jwt
            return Ok(tokenString);

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            //  user username create user instance
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };
            // hash password and save
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            // return
            return Ok();
        }






    }
}
