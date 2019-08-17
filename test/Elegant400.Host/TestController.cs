using Elegant400.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Elegant400.Host
{
   public class Post1
   {
      [Required]
      public string Value { get; set; }
   }

   public class Post2
   {
      [MinLength(5)]
      public string Value { get; set; }
   }

   public class Post3
   {
      [Required]
      public string Value { get; set; }
   }

   public class Post5
   {
      public int Value { get; set; }
   }

   [ApiController]
   [Route("/api/[controller]")]
   public class TestController : ControllerBase
   {
      [HttpPost("post1")]
      public IActionResult Post1(Post1 model) => Ok();

      [HttpPost("post2")]
      public IActionResult Post2(Post2 model) => Ok();

      [HttpPost("post3")]
      public IActionResult Post3([FromQuery]Post3 model) => Ok();

      [HttpPost("post4")]
      public IActionResult Post4()
      {
         var errors = new[]
         {
            new ValidationError("required", new[]{"value"}, new Dictionary<string, object> { {"length", 3 } })
         };
         var response = new ValidationResponse("Validation", errors);
         return BadRequest(response);
      }

      [HttpPost("post5")]
      public IActionResult Post5(Post5 model) => Ok();

   }
}
