using Elegant400.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Elegant400.Host
{
   public class NullObjectModelValidator : IObjectModelValidator
   {
      public void Validate(
          ActionContext actionContext,
          ValidationStateDictionary validationState,
          string prefix,
          object model)
      {
      }
   }

   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddMvc(cfg =>
         {
            cfg.Filters.Add<ValidationFilter>();
         }); //.AddElegant400();
         services.Configure<ApiBehaviorOptions>(options =>
         {
            options.SuppressModelStateInvalidFilter = true;
         });
         services.AddSingleton<IObjectModelValidator, NullObjectModelValidator>();
      }

      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
         app.UseMvc();
      }
   }
}
