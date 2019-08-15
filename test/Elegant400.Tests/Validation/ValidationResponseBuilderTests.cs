using Elegant400.Validation;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Blog.Tests.Validation
{
   public class ValidationResponseBuilderTests
   {
      public class RequiredModel
      {
         [Required]
         public string Summary { get; set; }
      }

      public class NestedModel
      {
         public RequiredModel Prop { get; set; }
      }

      public class NullableModel
      {
         [Required]
         public int? Value { get; set; }
      }

      public class EmptyCollectionModel
      {
         [Required]
         public IEnumerable<string> Values { get; set; }
      }

      public class MinLengthModel
      {
         [MinLength(3)]
         public string Name { get; set; }
      }

      public class ErrorAttribute : ValidationAttribute
      {
         public override bool IsValid(object value)
         {
            return false;
         }

         public string Error { get; set; }
      }

      public class ErrorAttributeModel
      {
         [Error]
         public string Value { get; set; }
      }

      public class PathAttribute : ValidationAttribute
      {
         public override bool IsValid(object value)
         {
            return false;
         }

         public string Path { get; set; }
      }

      public class PathAttributeModel
      {
         [Path]
         public string Value { get; set; }
      }

      public ValidationResponseBuilderTests()
      {
         builder = new ValidationResponseBuilder();
      }

      private readonly ValidationResponseBuilder builder;

      [Theory]
      [InlineData(null)]
      [InlineData("")]
      [InlineData("  ")]
      public void Check_required_string_properties(string value)
      {
         var model = new RequiredModel { Summary = value };

         builder.BuildFrom(model);

         builder.Invalid.Should().BeTrue();
         builder.Result.Errors.Should().ContainEquivalentOf(new
         {
            Error = "required",
            Path = new[] { "summary" }
         });
      }

      [Fact]
      public void Check_required_nullable_properties()
      {
         builder.BuildFrom(new NullableModel { Value = null });

         builder.Invalid.Should().BeTrue();
         builder.Result.Errors.Should().ContainEquivalentOf(new
         {
            Error = "required",
            Path = new[] { "value" }
         });
      }

      [Fact]
      public void Approve_required_properties()
      {
         var model = new RequiredModel { Summary = "text" };

         builder.BuildFrom(model);

         builder.Invalid.Should().BeFalse();
         builder.Result.Should().BeNull();
      }

      [Fact]
      public void Check_nested_properties()
      {
         var model = new NestedModel
         {
            Prop = new RequiredModel()
         };

         builder.BuildFrom(model);

         builder.Invalid.Should().BeTrue();
         builder.Result.Errors.Should().ContainEquivalentOf(new
         {
            Error = "required",
            Path = new[] { "prop", "summary" }
         });
      }

      [Fact]
      public void Return_null_if_every_property_is_ok()
      {
         var model = new RequiredModel { Summary = "text" };

         builder.BuildFrom(model);

         builder.Invalid.Should().BeFalse();
         builder.Result.Should().BeNull();
      }

      [Fact]
      public void Check_empty_collections()
      {
         builder.BuildFrom(new EmptyCollectionModel { Values = new string[] { } });

         builder.Invalid.Should().BeTrue();
         builder.Result.Errors.Should().BeEquivalentTo(new
         {
            Error = "empty",
            Path = new[] { "values" }
         });
      }

      [Fact]
      public void Check_min_length()
      {
         builder.BuildFrom(new MinLengthModel { Name = "12" });

         builder.Invalid.Should().BeTrue();
         builder.Result.Errors.Should().BeEquivalentTo(new
         {
            Error = "minLength",
            Path = new[] { "name" },
            Properties = new Dictionary<string, object> { { "length", 3 } }
         });
      }

      [Fact]
      public void Throw_if_property_is_named_error()
      {
         builder.Invoking(x => x.BuildFrom(new ErrorAttributeModel()))
            .Should()
            .Throw<InvalidOperationException>();
      }

      [Fact]
      public void Throw_if_property_is_named_path()
      {
         builder.Invoking(x => x.BuildFrom(new PathAttributeModel()))
            .Should()
            .Throw<InvalidOperationException>();
      }
   }
}
