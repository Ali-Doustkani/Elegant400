using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Elegant400.PropertyNavigation
{
   /// <summary>
   /// Represents a property.
   /// </summary>
   public class Property
   {
      public Property(IEnumerable<ValidationAttribute> attributes, object value, IEnumerable<object> path)
      {
         Attributes = attributes;
         Value = value;
         Path = path;
      }

      public IEnumerable<ValidationAttribute> Attributes { get; }
      public object Value { get; }
      public IEnumerable<object> Path { get; }
   }
}
