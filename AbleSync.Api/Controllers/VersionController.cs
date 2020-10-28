using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbleSync.Api.Controllers
{
    /// <summary>
    ///     Controller to get our version number.
    /// </summary>
    [AllowAnonymous]
    public class VersionController : ControllerBase
    {
        [HttpGet("version")]
        public IActionResult Get()
            => Ok(new
            {
                Name = "TODO Implement this",
                Version = 1337
            });
    }
}
