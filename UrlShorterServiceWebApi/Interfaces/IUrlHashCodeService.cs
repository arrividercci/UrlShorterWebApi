namespace UrlShorterServiceWebApi.Interfaces
{
    public interface IUrlHashCodeService
    {
        public Task<string> GetUrlHashCodeAsync(string url);
    }
}
