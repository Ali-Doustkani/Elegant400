using Elegant400.Utils;
using Elegant400.Validation;
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace Elegant400.Tests.Utils
{
   public class ValidationResponseConverterTests
   {
      private JToken Convert(ValidationResponse response) =>
         JToken.Parse(JsonConvert.SerializeObject(response, new ValidationResponseConverter()));

      [Fact]
      public void Set_properties_part_of_the_json()
      {
         var errors = new List<ValidationError>();
         errors.Add(new ValidationError("required", new object[] { "experiences", 0, "company" }));
         errors.Add(new ValidationError("minLength", new object[] { "summary" }, new Dictionary<string, object> { { "length", 3 } }));
         var response = new ValidationResponse("Validation", errors);

         var json = Convert(response);

         var expect = JsonConvert.SerializeObject(new
         {
            title = "Validation",
            errors = new object[]
            {
               new{error="required", path=new object[]{"experiences", 0, "company"}},
               new{error="minLength", path=new object[]{"summary"}, length=3}
            }
         });

         json.Should().BeEquivalentTo(expect);
      }

      [Fact]
      public void Convert_null()
      {
         var json = JsonConvert.SerializeObject(null, new ValidationResponseConverter());
         json.Should().Be("null");
      }
   }
}
