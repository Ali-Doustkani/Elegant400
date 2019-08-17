using Elegant400.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Elegant400
{
   public static class Builder
   {
      public static void AddElegant400(this IServiceCollection services)
      {
         services.Configure<MvcOptions>(options =>
         {
            options.Filters.Add<ValidationFilter>();
         });

         services.Configure<ApiBehaviorOptions>(options =>
         {
            options.SuppressModelStateInvalidFilter = true;
         });

         services.AddSingleton<IObjectModelValidator, NullObjectModelValidator>();
      }
   }
}
