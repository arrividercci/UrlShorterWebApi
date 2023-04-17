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

        public UrlController(UrlShorterContext context, IUrlHashCodeService urlHashCodeService, IUrlGeneratorService urlGeneratorService)
        {
            this.context = context;
            this.urlHashCodeService = urlHashCodeService;
            this.urlGeneratorService = urlGeneratorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Url>>> Get()
        {
            var urls = await context.Urls.ToListAsync();
            return Ok(urls);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Url>>> GetById(int id)
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
        public async Task<ActionResult> Add([FromBody] string originalUrl) 
        {
            try
            {
                Url url = new Url()
                {
                    OriginalUrl = originalUrl,
                    ShortUrl = urlGeneratorService.GetUrlByCode(urlHashCodeService.GetUrlHashCode(originalUrl)),
                    CreationDate = DateTime.Now,
                };

                await context.Urls.AddAsync(url);
                
                await context.SaveChangesAsync();
                
                return Ok(url.ShortUrl);
            } 
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] string originalUrl, [FromBody] string customUrl)
        {
            try
            {
                Url url = new Url()
                {
                    OriginalUrl = originalUrl,
                    ShortUrl = customUrl,
                    CreationDate = DateTime.Now,
                };

                await context.Urls.AddAsync(url);

                await context.SaveChangesAsync();

                return Ok($"api/url/{url.ShortUrl}");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] string customUrl)
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
                    url.ShortUrl = customUrl;
                    context.Urls.Update(url);
                    await context.SaveChangesAsync();
                    return Ok($"api/url/{url.ShortUrl}");
                }
            }
            catch (Exception) 
            {
                return BadRequest();
            }
        }

    }
}
