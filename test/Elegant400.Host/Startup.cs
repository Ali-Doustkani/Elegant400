using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Elegant400.Host
{
   public class Startup
   {
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddElegant400();
         services.AddMvc();
      }

      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
         app.UseMvc();
      }
   }
}
