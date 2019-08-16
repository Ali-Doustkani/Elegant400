using Elegant400.Validation;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Elegant400.Tests.Validation
{
   public class PropertiesExtractorTests
   {
      [Fact]
      public void Verify_Required() =>
         PropertiesExtractor.Extract(new RequiredAttribute())
         .Should()
         .HaveCount(0);

      [Fact]
      public void Verify_Compare() =>
         PropertiesExtractor.Extract(new CompareAttribute("TheOther"))
         .Should()
         .HaveCount(1)
         .And
         .Contain("otherProperty", "TheOther");

      [Fact]
      public void Verify_MaxLength() =>
         PropertiesExtractor.Extract(new MaxLengthAttribute(3))
         .Should()
         .HaveCount(1)
         .And
         .Contain("length", 3);

      [Fact]
      public void Verify_MinLength() =>
         PropertiesExtractor.Extract(new MinLengthAttribute(4))
         .Should()
         .HaveCount(1)
         .And
         .Contain("length", 4);

      [Fact]
      public void Verify_Range() =>
         PropertiesExtractor.Extract(new RangeAttribute(4, 6))
         .Should()
         .HaveCount(2)
         .And
         .Contain("minimum", 4)
         .And
         .Contain("maximum", 6);

      [Fact]
      public void Verify_RegularExpressions() =>
         PropertiesExtractor.Extract(new RegularExpressionAttribute("pattern"))
         .Should()
         .HaveCount(1)
         .And
         .Contain("pattern", "pattern");

      [Fact]
      public void Verify_StringLength() =>
         PropertiesExtractor.Extract(new StringLengthAttribute(5) { MinimumLength = 2 })
         .Should()
         .HaveCount(2)
         .And
         .Contain("minimumLength", 2)
         .And
         .Contain("maximumLength", 5);

      [Fact]
      public void Verify_CreditCard() =>
         PropertiesExtractor.Extract(new CreditCardAttribute())
         .Should()
         .HaveCount(0);

      [Fact]
      public void Verify_Email() =>
         PropertiesExtractor.Extract(new EmailAddressAttribute())
         .Should()
         .HaveCount(0);

      [Fact]
      public void Verify_EnumData() =>
         PropertiesExtractor.Extract(new EnumDataTypeAttribute(typeof(DataType)))
            .Should()
            .HaveCount(0);

      [Fact]
      public void Verify_FileExtension() =>
         PropertiesExtractor.Extract(new FileExtensionsAttribute() { Extensions = "jsx" })
         .Should()
         .HaveCount(1)
         .And
         .Contain("extensions", "jsx");

      [Fact]
      public void Verify_Phone() =>
         PropertiesExtractor.Extract(new PhoneAttribute())
         .Should()
         .HaveCount(0);

      [Fact]
      public void Verify_Url() =>
         PropertiesExtractor.Extract(new UrlAttribute())
         .Should()
         .HaveCount(0);
   }
}
