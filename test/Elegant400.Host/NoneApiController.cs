using Microsoft.AspNetCore.Mvc;

namespace Elegant400.Host
{
   [Route("home")]
   public class NoneApiController : Controller
   {
      [HttpPost("post")]
      public IActionResult Post() => Content("Done");
   }
}
