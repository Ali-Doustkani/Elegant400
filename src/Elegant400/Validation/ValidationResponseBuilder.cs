using Elegant400.PropertyNavigation;
using Elegant400.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Elegant400.Validation
{
   /// <summary>
   /// Navigates through all properties and checks all of them to create a list of ValidationError objects.
   /// </summary>
   public class ValidationResponseBuilder
   {
      private List<ValidationError> _types;

      public bool Invalid => _types.Any();

      public ValidationResponse Result =>
         Invalid ? new ValidationResponse("Validation", _types) : null;

      public void BuildFrom(object model)
      {
         _types = new List<ValidationError>();
         var navigator = new PropertyNavigator(model);
         while (navigator.Read())
            navigator.ForEachAttribute(CheckRequired);
      }

      private void CheckRequired(ValidationAttribute attrib, Property property)
      {
         if (SpecialCases(attrib, property))
            return;

         if (!attrib.IsValid(property.Value))
            _types.Add(new ValidationError(Map(attrib), property.Path, PropertiesExtractor.Extract(attrib)));
      }

      private string Map(ValidationAttribute attribute) =>
         attribute.GetType().Name.Replace("Attribute", "").ToCamelCase();

      private bool SpecialCases(ValidationAttribute attrib, Property property)
      {
         if (attrib is RequiredAttribute && property.Value is IEnumerable<object> list)
         {
            if (!list.Any())
               _types.Add(new ValidationError("empty", property.Path));
            return true;
         }
         return false;
      }
   }
}
