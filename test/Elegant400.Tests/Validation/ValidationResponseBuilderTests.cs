using Elegant400.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

         builder.BuildFromModel(model);

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
         builder.BuildFromModel(new NullableModel { Value = null });

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

         builder.BuildFromModel(model);

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

         builder.BuildFromModel(model);

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

         builder.BuildFromModel(model);

         builder.Invalid.Should().BeFalse();
         builder.Result.Should().BeNull();
      }

      [Fact]
      public void Check_empty_collections()
      {
         builder.BuildFromModel(new EmptyCollectionModel { Values = new string[] { } });

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
         builder.BuildFromModel(new MinLengthModel { Name = "12" });

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
         builder.Invoking(x => x.BuildFromModel(new ErrorAttributeModel()))
            .Should()
            .Throw<InvalidOperationException>();
      }

      [Fact]
      public void Throw_if_property_is_named_path()
      {
         builder.Invoking(x => x.BuildFromModel(new PathAttributeModel()))
            .Should()
            .Throw<InvalidOperationException>();
      }

      [Fact]
      public void Build_from_model_state_for_integer_type()
      {
         var modelState = new Dictionary<string, ModelStateEntry>();
         var entry = Mock.Of<ModelStateEntry>();
         entry.Errors.Add(new JsonReaderException("Could not convert string to integer"));
         modelState.Add("value", entry);
         builder.BuildFromModelState(modelState);

         builder.Invalid.Should().BeTrue();
         builder.Result.Errors.Should().BeEquivalentTo(new
         {
            Error = "convert",
            Path = new[] { "value" },
            Properties = new Dictionary<string, object> { { "type", "integer" } }
         });
      }

      [Fact]
      public void Build_from_model_state_for_date_type()
      {
         var modelState = new Dictionary<string, ModelStateEntry>();
         var entry = Mock.Of<ModelStateEntry>();
         entry.Errors.Add(new JsonReaderException("Could not convert string to date"));
         modelState.Add("value", entry);
         builder.BuildFromModelState(modelState);

         builder.Invalid.Should().BeTrue();
         builder.Result.Errors.Should().BeEquivalentTo(new
         {
            Error = "convert",
            Path = new[] { "value" },
            Properties = new Dictionary<string, object> { { "type", "date" } }
         });
      }

      [Fact]
      public void Build_from_model_state_with_multiple_errors()
      {
         var modelState = new Dictionary<string, ModelStateEntry>();
         var entry1 = Mock.Of<ModelStateEntry>();
         entry1.Errors.Add(new JsonReaderException("Could not convert string to integer"));
         modelState.Add("age", entry1);
         var entry2 = Mock.Of<ModelStateEntry>();
         entry2.Errors.Add(new JsonReaderException("Could not convert string to date"));
         modelState.Add("startDate", entry2);
         builder.BuildFromModelState(modelState);

         builder.Invalid.Should().BeTrue();
         builder.Result.Errors.Should().BeEquivalentTo(new[] {
            new
            {
               Error = "convert",
               Path = new[] {"age"},
               Properties = new Dictionary<string, object> { { "type", "integer"} }
            },
            new
            {
               Error = "convert",
               Path = new[]{"startDate" },
               Properties = new Dictionary<string,object>{{"type", "date"}}
            }
         });
      }

      [Fact]
      public void Process_keys_to_path()
      {
         var modelState = new Dictionary<string, ModelStateEntry>();
         var entry = Mock.Of<ModelStateEntry>();
         entry.Errors.Add(new JsonReaderException("Could not convert string to integer"));
         modelState.Add("person.surname", entry);
         builder.BuildFromModelState(modelState);

         builder.Result.Errors.First().Path.Should().BeEquivalentTo(new[] { "person", "surname" });
      }

      [Fact]
      public void Process_keys_containing_arrays()
      {
         var modelState = new Dictionary<string, ModelStateEntry>();
         var entry = Mock.Of<ModelStateEntry>();
         entry.Errors.Add(new JsonReaderException("Could not convert string to integer"));
         modelState.Add("people[1].surname", entry);
         builder.BuildFromModelState(modelState);

         builder.Result.Errors.First().Path.Should().BeEquivalentTo(new object[] { "people", 1, "surname" });
      }
   }
}
