using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShorterServiceWebApi.Data;

namespace UrlShorterServiceWebApi.Controllers
{
    [Route("ushorter")]
    [ApiController]
    public class UrlRedirectorController : ControllerBase
    {
        private readonly UrlShorterContext context;
        public UrlRedirectorController(UrlShorterContext context)
        {
            this.context = context;
        }

        [HttpGet("{shortUrl}")]
        public async Task<ActionResult> GetByShortUrl(string shortUrl)
        {
            if (ModelState.IsValid)
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
            else
            {
                return BadRequest();
            }
        }
    }
}
