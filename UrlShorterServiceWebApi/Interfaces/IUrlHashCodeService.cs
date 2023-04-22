namespace UrlShorterServiceWebApi.Interfaces
{
    public interface IUrlHashCodeService
    {
        public string GetUrlHashCode(string url);
    }
}
