using Elegant400.PropertyNavigation;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Blog.Tests.Validation
{
   public class PropertyNavigatorTests
   {
      public class NotValidation : Attribute { }

      public class Flat1
      {
         [NotValidation]
         [Required]
         [MaxLength(3)]
         public int First { get; set; }
      }

      public class Flat2
      {
         [Required]
         public int First { get; set; }

         public string Second { get; set; }
      }

      public class Nested1
      {
         [Required]
         public string Zero { get; set; }

         public string Zero2 { get; set; }

         public Flat2 NestedModel1 { get; set; }

         [Required]
         public string Third { get; set; }

         [Required]
         public Flat2 NestedModel2 { get; set; }
      }

      public class CamelCase
      {
         [Required]
         public string ThisShouldBeCamelCase { get; set; }
      }

      public class NullableModel
      {
         [Required]
         public int? Value { get; set; }
      }

      public class ClassProperty
      {
         [Required]
         public NullableModel Nullable { get; set; }
      }

      public class CollectionProperty
      {
         public IEnumerable<object> Flats { get; set; }
      }

      public class NullProperty
      {
         public Flat1 TheProperty { get; set; }
      }

      private List<Property> Read(object model)
      {
         var nav = new PropertyNavigator(model);
         var result = new List<Property>();
         while (nav.Read())
            result.Add(nav.Property);
         return result;
      }

      [Fact]
      public void Read_properties_from_flat_class()
      {
         var model = new Flat1 { First = 12 };
         var result = Read(model);

         result.Should().HaveCount(1);
         result[0].Attributes.Should().HaveCount(2);
         result[0].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[0].Attributes.ElementAt(1).Should().BeAssignableTo<MaxLengthAttribute>();
         result[0].Value.Should().Be(12);
         result[0].Path.Should().BeEquivalentTo(new[] { "first" });
      }

      [Fact]
      public void Read_only_properties_with_attributes()
      {
         var model = new Flat2 { First = 12, Second = "Ali" };
         var result = Read(model);

         result.Should().HaveCount(1);
         result[0].Attributes.Should().HaveCount(1);
         result[0].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[0].Value.Should().Be(12);
         result[0].Path.Should().BeEquivalentTo(new[] { "first" });
      }

      [Fact]
      public void Make_path_strings_camel_case()
      {
         var result = Read(new CamelCase());

         result[0].Path.Should().BeEquivalentTo(new[] { "thisShouldBeCamelCase" });
      }

      [Fact]
      public void Read_properties_of_nested_types()
      {
         var model = new Nested1
         {
            Zero = "zero",
            NestedModel1 = new Flat2 { First = 11, Second = "val-11" },
            Third = "third",
            NestedModel2 = new Flat2 { First = 22, Second = "val-22" }
         };
         var result = Read(model);

         result.Should().HaveCount(5);

         result[0].Attributes.Should().HaveCount(1);
         result[0].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[0].Value.Should().Be("zero");
         result[0].Path.Should().BeEquivalentTo(new[] { "zero" });

         result[1].Attributes.Should().HaveCount(1);
         result[1].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[1].Value.Should().Be(11);
         result[1].Path.Should().BeEquivalentTo(new[] { "nestedModel1", "first" });

         result[2].Attributes.Should().HaveCount(1);
         result[2].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[2].Value.Should().Be("third");
         result[2].Path.Should().BeEquivalentTo(new[] { "third" });

         result[3].Attributes.Should().HaveCount(1);
         result[3].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[3].Value.Should().Be(model.NestedModel2);
         result[3].Path.Should().BeEquivalentTo(new[] { "nestedModel2" });

         result[4].Attributes.Should().HaveCount(1);
         result[4].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[4].Value.Should().Be(22);
         result[4].Path.Should().BeEquivalentTo(new[] { "nestedModel2", "first" });
      }

      [Fact]
      public void Read_null_properties()
      {
         var result = Read(new NullProperty());

         result.Should().HaveCount(0);
      }

      [Fact]
      public void Read_nullable_primitive_types()
      {
         var model = new NullableModel { Value = null };
         var result = Read(model);

         result.Should().HaveCount(1);

         result[0].Attributes.Should().HaveCount(1);
         result[0].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[0].Value.Should().BeNull();
         result[0].Path.Should().BeEquivalentTo(new[] { "value" });
      }

      [Fact]
      public void Read_class_properties()
      {
         var val = new NullableModel { Value = 12 };
         var model = new ClassProperty { Nullable = val };
         var result = Read(model);

         result.Should().HaveCount(2);

         result[0].Attributes.Should().HaveCount(1);
         result[0].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[0].Value.Should().Be(val);
         result[0].Path.Should().BeEquivalentTo(new[] { "nullable" });

         result[1].Attributes.Should().HaveCount(1);
         result[1].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[1].Value.Should().Be(12);
         result[1].Path.Should().BeEquivalentTo(new[] { "nullable", "value" });
      }

      [Fact]
      public void Read_collections()
      {
         var model = new CollectionProperty
         {
            Flats = new object[]
            {
               new Flat2{ First=1, Second="First"},
               new CamelCase{ ThisShouldBeCamelCase="" }
            }
         };
         var result = Read(model);

         result.Should().HaveCount(2);

         result[0].Attributes.Should().HaveCount(1);
         result[0].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[0].Value.Should().Be(1);
         result[0].Path.Should().BeEquivalentTo(new object[] { "flats", 0, "first" });

         result[1].Attributes.Should().HaveCount(1);
         result[1].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[1].Value.Should().Be("");
         result[1].Path.Should().BeEquivalentTo(new object[] { "flats", 1, "thisShouldBeCamelCase" });
      }

      [Fact]
      public void Read_null_collections()
      {
         var model = new CollectionProperty { Flats = null };
         var result = Read(model);

         result.Should().HaveCount(0);
      }

      [Fact]
      public void Read_only_validation_attributes()
      {
         var result = Read(new Flat1());

         result.Should().HaveCount(1);
         result[0].Attributes.Should().HaveCount(2);
         result[0].Attributes.ElementAt(0).Should().BeAssignableTo<RequiredAttribute>();
         result[0].Attributes.ElementAt(1).Should().BeAssignableTo<MaxLengthAttribute>();
      }
   }
}
