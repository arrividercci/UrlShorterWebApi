using System.Text;

namespace UrlShorterServiceWebApi.Services
{
    public class UrlHashCodeService
    {
        public int GetUrlHashCode(string url)
        {
            return Math.Abs(url.GetHashCode());   
        }
    }
}
