namespace UrlShorterServiceWebApi.Interfaces
{
    public interface IUrlGeneratorService
    {
        public string GetUrlByCode(string mapString);
    }
}
