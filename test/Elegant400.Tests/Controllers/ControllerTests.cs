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
      public async Task Validate_required_criteria()
      {
         using (var response = await _client.PostAsJsonAsync("/api/test/post1", new { }))
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

      [Fact]
      public async Task Validate_minLength_criteria()
      {
         using (var response = await _client.PostAsJsonAsync("/api/test/post2", new { Value = "abc" }))
         {
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = JToken.Parse(await response.Content.ReadAsStringAsync());
            var expect = JsonConvert.SerializeObject(new
            {
               title = "Validation",
               errors = new[]
               {
                  new {error="minLength", path=new[]{"value"}, length=5}
               }
            });

            result.Should().BeEquivalentTo(expect);
         }
      }

      [Fact]
      public async Task Dont_check_none_api_controller_requests()
      {
         using (var response = await _client.PostAsJsonAsync("/home/post", new { }))
         {
            response.EnsureSuccessStatusCode();
            (await response.Content.ReadAsStringAsync()).Should().Be("Done");
         }
      }

      [Fact]
      public async Task Validate_query_strings()
      {
         var req = new HttpRequestMessage(HttpMethod.Post, "/api/test/post3?value=sth");
         using (var response = await _client.SendAsync(req))
            response.EnsureSuccessStatusCode();

         using (var response = await _client.PostAsJsonAsync("/api/test/post3", new { }))
         {
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result = JToken.Parse(await response.Content.ReadAsStringAsync());
            var expect = JsonConvert.SerializeObject(new
            {
               title = "Validation",
               errors = new[]
               {
                  new {error="required", path=new[]{"value"}}
               }
            });

            result.Should().BeEquivalentTo(expect);
         }
      }

      [Fact]
      public async Task Validate_controller_returned_errors()
      {
         var req = new HttpRequestMessage(HttpMethod.Post, "/api/test/post4");
         using (var response = await _client.SendAsync(req))
         {
            var result = JToken.Parse(await response.Content.ReadAsStringAsync());
            var expected = JsonConvert.SerializeObject(new
            {
               title = "Validation",
               errors = new[]
               {
                  new{error="required", path=new[]{"value"}, length=3},
               }
            });

            result.Should().BeEquivalentTo(expected);
         }
      }

      [Fact]
      public async Task Validate_not_convertable_inputs()
      {
         using (var response = await _client.PostAsJsonAsync("/api/test/post5", new { value = "abc" }))
         {
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = JToken.Parse(await response.Content.ReadAsStringAsync());
            var expected = JsonConvert.SerializeObject(new
            {
               title = "Validation",
               errors = new[]
               {
                  new{error="convert", path=new[]{"value"}, type="integer" }
               }
            });
            result.Should().BeEquivalentTo(expected);
         }
      }
   }
}
