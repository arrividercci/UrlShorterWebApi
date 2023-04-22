using System.Security.Cryptography;
using System.Text;
using UrlShorterServiceWebApi.Interfaces;

namespace UrlShorterServiceWebApi.Services
{
    public class UrlHashCodeService : IUrlHashCodeService
    {
        public string GetUrlHashCode(string url)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(url));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2"));
            }
            string hexString = sb.ToString();
            List<char> letters = new List<char>();
            List<char> digits = new List<char>();
            foreach (char c in hexString)
            {
                if (char.IsLetter(c))
                {
                    letters.Add(c);
                }
                else if (char.IsDigit(c))
                {
                    digits.Add(c);
                }
            }
            string hashString = new string(letters.ToArray()) + new string(digits.ToArray());
            return hashString;
        }
    }
}
