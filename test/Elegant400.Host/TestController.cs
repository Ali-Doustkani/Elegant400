using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Elegant400.Host
{
   public class ViewModel
   {
      [Required]
      [EmailAddress]
      public string Value { get; set; }
   }

   [ApiController]
   [Route("/api/[controller]")]
   public class TestController : ControllerBase
   {
      [HttpPost]
      public IActionResult Post(ViewModel model)
      {
         return Ok();
      }
   }
}
