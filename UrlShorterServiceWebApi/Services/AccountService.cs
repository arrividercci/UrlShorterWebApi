using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt").GetSection("Key").Value));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: configuration.GetSection("Jwt").GetSection("Issuer").Value,
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(configuration.GetSection("Jwt").GetSection("Lifetime").Value)),
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
