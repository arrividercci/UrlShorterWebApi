using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShorterServiceWebApi.Data;
using UrlShorterServiceWebApi.Entities;
using UrlShorterServiceWebApi.Interfaces;
using UrlShorterServiceWebApi.Models;
using UrlShorterServiceWebApi;
using System.Linq;
using System.Security.Claims;

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
        [Authorize(Roles = RolesString.Admin)]
        public async Task<ActionResult<IEnumerable<UrlModel>>> Get()
        {
            var urls = await context.Urls.ToListAsync();
            var urlsDto = new List<UrlModel>();
            foreach(var url in urls)
            {
                var urlDto = new UrlModel()
                {
                    OriginalUrl = url.OriginalUrl,
                    ShortUrl = url.ShortUrl
                };
                urlsDto.Add(urlDto);
            }
            return Ok(urlsDto);
        }

        [HttpGet("my")]
        [Authorize(Roles = RolesString.User)]
        public async Task<ActionResult<IEnumerable<UrlModel>>> GetMyUrls()
        {
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userId == null) return BadRequest();
            var urlsIds = await context.UserUrls
                .Where(userUrl => userUrl.Equals(userId))
                .Select(userUrl => userUrl.UrlId)
                .ToListAsync();
            
            var urls = await context.Urls
                .Where(url => urlsIds.Contains(url.Id))
                .ToListAsync();
            
            var urlsDto = new List<UrlModel>();
            
            foreach (var url in urls)
            {
                var urlDto = new UrlModel()
                {
                    OriginalUrl = url.OriginalUrl,
                    ShortUrl = url.ShortUrl
                };
                urlsDto.Add(urlDto);
            }
            return Ok(urlsDto);
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = RolesString.Admin)]
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
        public async Task<ActionResult<UrlDto>> Add([FromBody] UrlDto urlDto)
        {
            try
            {
                var originalUrl = urlDto.Url;
                if(string.IsNullOrEmpty(originalUrl)) return BadRequest();
                var BaseUrl = configuration.GetSection(SettingStrings.ServicesUrlsSection).GetSection(SettingStrings.UrlsApi).Value;
                var url = new Url();
                url.OriginalUrl = originalUrl;
                url.ShortUrl = urlGeneratorService.GetUrlByCode(urlHashCodeService.GetUrlHashCode(url.OriginalUrl));
                url.CreationDate = DateTime.Now;
                await context.AddAsync(url);
                await context.SaveChangesAsync();
                urlDto.Url = $"{BaseUrl}api/url/{url.ShortUrl}";
                return Ok(urlDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("custom")]
        [Authorize(Roles = RolesString.User)]
        public async Task<ActionResult<UrlDto>> Add([FromBody] UrlModel urlModel)
        {
            try
            {
                if (string.IsNullOrEmpty(urlModel.OriginalUrl) || string.IsNullOrEmpty(urlModel.ShortUrl)) return BadRequest();
                var BaseUrl = configuration.GetSection(SettingStrings.ServicesUrlsSection).GetSection(SettingStrings.UrlsApi).Value;
                var url = new Url();
                url.OriginalUrl = urlModel.OriginalUrl;
                url.ShortUrl = urlModel.ShortUrl;
                url.CreationDate = DateTime.Now;
                await context.AddAsync(url);
                await context.SaveChangesAsync();
                var userId = User.FindFirst(ClaimTypes.Name)?.Value;
                if (userId == null) return BadRequest();
                var userUrl = new UserUrls()
                {
                    UserId = userId,
                    UrlId = url.Id
                };
                await context.AddAsync(userUrl);
                await context.SaveChangesAsync();
                var urlDto = new UrlDto()
                {
                    Url = $"{BaseUrl}api/url/{url.ShortUrl}"
                };
                return Ok(urlDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RolesString.Admin + ", " + RolesString.User)]
        public async Task<ActionResult> Update(int id, [FromBody] UrlModel urlModel)
        {
            try
            {
                if (urlModel == null)
                {
                    return BadRequest();
                }
                else
                {
                    var url = await context.Urls.FirstOrDefaultAsync(url => url.Id == id);
                    if (url == null) return NotFound();
                    url.ShortUrl = urlModel.ShortUrl;
                    url.OriginalUrl = urlModel.OriginalUrl;
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
        [Authorize(Roles = RolesString.User + ", " + RolesString.Admin)]
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
