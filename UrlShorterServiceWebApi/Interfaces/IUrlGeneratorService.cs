namespace UrlShorterServiceWebApi.Interfaces
{
    public interface IUrlGeneratorService
    {
        public string GetUrlByCode(int code);
    }
}
