namespace UrlShorterServiceWebApi.Interfaces
{
    public interface IUrlGeneratorService
    {
        public Task<string> GetUrlByCodeAsync(string mapString);
    }
}
