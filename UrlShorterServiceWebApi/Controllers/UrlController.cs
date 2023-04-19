using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShorterServiceWebApi.Data;
using UrlShorterServiceWebApi.Entities;
using UrlShorterServiceWebApi.Interfaces;
using UrlShorterServiceWebApi.Services;


namespace UrlShorterServiceWebApi.Controllers
{
    [Route("api/url")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly UrlShorterContext context;
        private readonly IUrlHashCodeService urlHashCodeService;
        private readonly IUrlGeneratorService urlGeneratorService;
        private readonly IConfiguration configuration;

        public UrlController(UrlShorterContext context, IUrlHashCodeService urlHashCodeService, IUrlGeneratorService urlGeneratorService, IConfiguration configuration)
        {
            this.context = context;
            this.urlHashCodeService = urlHashCodeService;
            this.urlGeneratorService = urlGeneratorService;
            this.configuration = configuration;
            
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Url>>> Get()
        {
            var urls = await context.Urls.ToListAsync();
            return Ok(urls);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Url>>> Get(int id)
        {
            var url = await context.Urls.FirstOrDefaultAsync(url => url.Id == id);
            if (url == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(url);
            }
        }

        [HttpGet("{shortUrl}")]
        public async Task<ActionResult> GetByShortUrl(string shortUrl)
        {
            var url = await context.Urls.FirstOrDefaultAsync(url => url.ShortUrl.Equals(shortUrl));
            if (url == null)
            {
                return NotFound();
            }
            else
            {
                return Redirect(url.OriginalUrl);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Url url) 
        {
            try
            {
                var BaseUrl = configuration.GetSection(SettingStrings.ServicesUrlsSection).GetSection(SettingStrings.UrlsApi).Value;
                if (string.IsNullOrEmpty(url.ShortUrl))
                {
                    url.ShortUrl = urlGeneratorService.GetUrlByCode(urlHashCodeService.GetUrlHashCode(url.OriginalUrl));
                }
                url.CreationDate = DateTime.Now;
                await context.AddAsync(url);
                await context.SaveChangesAsync();
                return Ok($"{BaseUrl}api/url/{url.ShortUrl}");
            } 
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Url url)
        {
            try
            {
                if (url == null)
                {
                    return NotFound();
                }
                else
                {
                    context.Urls.Update(url);
                    await context.SaveChangesAsync();
                    return NoContent();
                }
            }
            catch (Exception) 
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var url = await context.Urls.FirstOrDefaultAsync(url => url.Id == id);
                if (url == null)
                {
                    return NotFound();
                }
                else
                {
                    context.Remove(url);
                    await context.SaveChangesAsync();
                    return NoContent();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
