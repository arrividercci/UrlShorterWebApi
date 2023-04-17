namespace UrlShorterServiceWebApi.Interfaces
{
    public interface IUrlHashCodeService
    {
        public int GetUrlHashCode(string url);
    }
}
