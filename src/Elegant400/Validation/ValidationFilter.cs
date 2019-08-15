using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Elegant400.Validation
{
   public class ValidationFilter : ActionFilterAttribute
   {

      public override void OnActionExecuting(ActionExecutingContext context)
      {
         if (!context.Filters.Any(x => x.GetType() == typeof(ApiControllerAttribute)))
            return;

         if (context.ActionArguments.Count == 0)
            return;

         if (context.ActionArguments.Count > 1)
            throw new InvalidOperationException("Current validation only supports one argument");

         var builder = new ValidationResponseBuilder();
         builder.BuildFrom(context.ActionArguments.Single().Value);
         if (builder.Invalid)
            context.Result = new BadRequestObjectResult(builder.Result);
      }
   }
}
