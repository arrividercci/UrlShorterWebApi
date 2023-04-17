using System.Text;
using UrlShorterServiceWebApi.Interfaces;

namespace UrlShorterServiceWebApi.Services
{
    public class UrlHashCodeService : IUrlHashCodeService
    {
        public int GetUrlHashCode(string url)
        {
            return Math.Abs(url.GetHashCode());   
        }
    }
}
