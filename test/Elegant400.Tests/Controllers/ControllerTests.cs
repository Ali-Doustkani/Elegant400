using Elegant400.Host;
using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Elegant400.Tests.Controllers
{
   public class ControllerTests
   {
      public ControllerTests()
      {
         var testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
         _client = testServer.CreateClient();
      }

      private readonly HttpClient _client;

      [Fact]
      public async Task Validate_flat_objects()
      {
         using (var response = await _client.PostAsJsonAsync("/api/test", new { }))
         {
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = JToken.Parse(await response.Content.ReadAsStringAsync());
            var expect = JsonConvert.SerializeObject(new
            {
               title = "Validation",
               errors = new[]
               {
                  new{error="required", path=new[]{"value"} }
               }
            });
            result.Should().BeEquivalentTo(expect);
         }
      }
   }
}
