using System.Text;

namespace UrlShorterServiceWebApi.Services
{
    public class UrlGeneratorService
    {
        public string GetUrlByCode(int code)
        {
            var map = "0123456789qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            var size = map.Length;
            StringBuilder url = new StringBuilder();
            while(code >= 0)
            {
                url.Append(map[code % size]);
                code /= size;
            }
            return url.ToString();
        }
    }
}
