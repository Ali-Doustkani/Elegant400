using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Elegant400.Validation
{
   public class ValidationFilter : ActionFilterAttribute
   {
      public override void OnActionExecuting(ActionExecutingContext context)
      {
         if (!context.Filters.Any(x => x.GetType() == typeof(ApiControllerAttribute)))
            return;

         if (context.ActionDescriptor.Parameters.Count == 0)
            return;

         if (context.ActionDescriptor.Parameters.Count > 1)
            if (context.ModelState.IsValid)
               return;

         var builder = new ValidationResponseBuilder();

         if (context.ModelState.ValidationState == ModelValidationState.Invalid)
         {
            builder.BuildFromModelState(context.ModelState);
            context.Result = new BadRequestObjectResult(builder.Result);
            return;
         }

         builder.BuildFromModel(context.ActionArguments.Single().Value);
         if (builder.Invalid)
            context.Result = new BadRequestObjectResult(builder.Result);
      }
   }
}
