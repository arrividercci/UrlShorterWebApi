using System.Text;
using UrlShorterServiceWebApi.Interfaces;

namespace UrlShorterServiceWebApi.Services
{
    public class UrlGeneratorService : IUrlGeneratorService
    {
        public string GetUrlByCode(int code)
        {
            var mapString = "0123456789qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            var size = mapString.Length;
            StringBuilder url = new StringBuilder();
            while(code >= 0)
            {
                url.Append(mapString[code % size]);
                code /= size;
            }
            return url.ToString();
        }
    }
}
