using System.Text;
using UrlShorterServiceWebApi.Interfaces;

namespace UrlShorterServiceWebApi.Services
{
    public class UrlGeneratorService : IUrlGeneratorService
    {
        public async Task<string> GetUrlByCodeAsync(string mapString)
        {
            return await Task<string>.Run(() =>
            {
                var size = mapString.Length;
                var random = new Random();
                var url = new StringBuilder();
                for (int i = 0; i < 8; i++)
                {
                    url.Append(mapString[random.Next(size)]);
                }
                return url.ToString();
            });
        }
    }
}
