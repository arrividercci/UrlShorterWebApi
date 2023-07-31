using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShorterServiceWebApi.Data;
using UrlShorterServiceWebApi.Entities;
using UrlShorterServiceWebApi.Interfaces;
using UrlShorterServiceWebApi.Models;
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
        private readonly string BaseUrl;

        public UrlController(UrlShorterContext context, IUrlHashCodeService urlHashCodeService, IUrlGeneratorService urlGeneratorService, IConfiguration configuration)
        {
            this.context = context;
            this.urlHashCodeService = urlHashCodeService;
            this.urlGeneratorService = urlGeneratorService;
            this.configuration = configuration;
            this.BaseUrl = configuration.GetSection(SettingStrings.ServicesUrlsSection).GetSection(SettingStrings.UrlsApi).Value!;
        }

        [Authorize(Roles = RolesString.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrlToGet>>> Get()
        {
            
            var urls = await context.Urls.ToListAsync();
            var urlsToGet = new List<UrlToGet>();
            foreach (var url in urls)
            {
                var urlToGet = new UrlToGet()
                {
                    Id = url.Id,
                    OriginalUrl = url.OriginalUrl,
                    ShortUrl = $"{BaseUrl}ushorter/{url.ShortUrl}",
                    CodeUrl = url.ShortUrl
                };
                urlsToGet.Add(urlToGet);
            }
            return Ok(urlsToGet);
             
        }

        [HttpGet("my")]
        [Authorize(Roles = RolesString.User)]
        public async Task<ActionResult<IEnumerable<UrlToGet>>> GetMyUrls()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest();
            var urlsIds = await context.UserUrls
                .Where(userUrl => userUrl.UserId.Equals(userId))
                .Select(userUrl => userUrl.UrlId)
                .ToListAsync();

            var urls = await context.Urls
                .Where(url => urlsIds.Contains(url.Id))
                .ToListAsync();

            var urlsToGet = new List<UrlToGet>();

            foreach (var url in urls)
            {
                var urlToGet = new UrlToGet()
                {
                    Id = url.Id,
                    OriginalUrl = url.OriginalUrl,
                    ShortUrl = $"{BaseUrl}ushorter/{url.ShortUrl}",
                    CodeUrl = url.ShortUrl
                };
                urlsToGet.Add(urlToGet);
            }
            return Ok(urlsToGet);
             
        }

        [HttpGet("{id}")]
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

        [HttpPost]
        public async Task<ActionResult<UrlDto>> Add([FromBody] UrlDto urlDto)
        {
            try
            {
                var originalUrl = urlDto.Url;
                if (string.IsNullOrEmpty(originalUrl)) return BadRequest();
                var url = new Url();
                url.OriginalUrl = originalUrl;
                url.ShortUrl = await urlGeneratorService.GetUrlByCodeAsync(await urlHashCodeService.GetUrlHashCodeAsync(url.OriginalUrl));
                do
                {
                    url.ShortUrl = await urlGeneratorService.GetUrlByCodeAsync(await urlHashCodeService.GetUrlHashCodeAsync(url.OriginalUrl));
                } 
                while (await context.Urls.FirstOrDefaultAsync(u => u.ShortUrl.Equals(url.ShortUrl)) != null);
                url.CreationDate = DateTime.Now;
                await context.AddAsync(url);
                await context.SaveChangesAsync();
                urlDto.Url = $"{BaseUrl}ushorter/{url.ShortUrl}";
                return Ok(urlDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        

        [HttpPost("custom")]
        [Authorize(Roles = RolesString.User)]
        public async Task<ActionResult<UrlDto>> Add([FromBody] UrlModel urlModel)
        {
            
            try
            {
                if (string.IsNullOrEmpty(urlModel.OriginalUrl)) return BadRequest();
                
                var url = new Url();

                if (string.IsNullOrEmpty(urlModel.ShortUrl))
                {
                    do
                    {
                        url.ShortUrl = await urlGeneratorService.GetUrlByCodeAsync(await urlHashCodeService.GetUrlHashCodeAsync(urlModel.OriginalUrl));
                    } 
                    while (await context.Urls.FirstOrDefaultAsync(u => u.ShortUrl.Equals(urlModel.ShortUrl)) != null);
                }
                else
                {
                    if (await context.Urls.FirstOrDefaultAsync(u => u.ShortUrl.Equals(urlModel.ShortUrl)) != null) return BadRequest("Url code is already taken");
                    url.ShortUrl = urlModel.ShortUrl!;
                }
                url.OriginalUrl = urlModel.OriginalUrl;
                url.CreationDate = DateTime.Now;
                await context.AddAsync(url);
                await context.SaveChangesAsync();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return BadRequest();
                var userUrl = new UserUrls()
                {
                    UserId = userId,
                    UrlId = url.Id
                };
                await context.AddAsync(userUrl);
                await context.SaveChangesAsync();
                var urlDto = new UrlDto();
                urlDto.Url = $"{BaseUrl}ushorter/{url.ShortUrl}";
                return Ok(urlDto);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RolesString.User + "," + RolesString.Admin)]
        public async Task<ActionResult> Update(int id, [FromBody] UrlDto urlModel)
        {
            try
            {
                if (urlModel == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (await context.Urls.FirstOrDefaultAsync(u => u.ShortUrl.Equals(urlModel.Url)) != null) return BadRequest("Url code is already taken");
                    var url = await context.Urls.FirstOrDefaultAsync(url => url.Id == id);
                    if (url == null) return NotFound();
                    url.ShortUrl = urlModel.Url!;
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
        [Authorize(Roles = RolesString.User + "," + RolesString.Admin)]
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
