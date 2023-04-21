using UrlShorterServiceWebApi.Models;

namespace UrlShorterServiceWebApi.Interfaces
{
    public interface IAccountService
    {
        public Task Register(RegisterUserModel registerUserModel);
        public Task<string> Login(LoginUserModel loginUserModel);
    }
}
