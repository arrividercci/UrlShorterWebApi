using System.Security.Cryptography;
using System.Text;
using UrlShorterServiceWebApi.Interfaces;

namespace UrlShorterServiceWebApi.Services
{
    public class UrlHashCodeService : IUrlHashCodeService
    {
        public async Task<string> GetUrlHashCodeAsync(string url)
        {
            return await Task<string>.Run(() =>
            {
                MD5 md5Hash = MD5.Create();
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(url));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            });
        }
    }
}
