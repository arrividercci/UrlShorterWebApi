using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrlShorterServiceWebApi.Entities;
using UrlShorterServiceWebApi.Interfaces;
using UrlShorterServiceWebApi.Models;

namespace UrlShorterServiceWebApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        public AccountService(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task<string> Login(LoginUserModel loginUserModel)
        {
            if(await ValidateUser(loginUserModel))
            {
                var user = await userManager.FindByNameAsync(loginUserModel.Email);
                return await CreateJwtToken(user);
            }
            else
            {
                throw new Exception("Can't login");
            }    
        }

        public async Task Register(RegisterUserModel registerUserModel)
        {
            var user = new User()
            {
                Email = registerUserModel.Email,
                UserName = registerUserModel.Email
            };
            var result = await userManager.CreateAsync(user, registerUserModel.Password);
            if (result == null)
            {
                throw new Exception("Can't register");
            }
            await userManager.AddToRoleAsync(user, RolesString.User);
        }

        private async Task<bool> ValidateUser(LoginUserModel loginUserModel)
        {
            var user = await userManager.FindByNameAsync(loginUserModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, loginUserModel.Password)) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<string> CreateJwtToken(User user)
        {
            var myissuer = configuration["JwtSettings:Issuer"];
            var myaudience = configuration["JwtSettings:Audience"];
            var secretKey = configuration["JwtSettings:Key"];

            var userRoles = await userManager.GetRolesAsync(user);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtToken = new JwtSecurityToken(
                issuer: myissuer,
                audience: myaudience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            );

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            return jwtTokenHandler.WriteToken(jwtToken);
        }
    }
}
