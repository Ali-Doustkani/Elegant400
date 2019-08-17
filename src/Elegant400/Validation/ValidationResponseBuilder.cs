using Elegant400.PropertyNavigation;
using Elegant400.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Elegant400.Validation
{
   /// <summary>
   /// Navigates through all properties and checks all of them to create a list of ValidationError objects.
   /// </summary>
   public class ValidationResponseBuilder
   {
      private List<ValidationError> _errors;

      public bool Invalid => _errors.Any();

      public ValidationResponse Result =>
         Invalid ? new ValidationResponse("Validation", _errors) : null;

      public void BuildFromModel(object model)
      {
         _errors = new List<ValidationError>();
         var navigator = new PropertyNavigator(model);
         while (navigator.Read())
            navigator.ForEachAttribute(Validate);
      }

      public void BuildFromModelState(IReadOnlyCollection<KeyValuePair<string, ModelStateEntry>> modelState)
      {
         _errors = new List<ValidationError>();
         foreach (var state in modelState)
            _errors.Add(new ValidationError("convert", ToPath(state.Key), ToProperties(state.Value)));
      }

      private IEnumerable<object> ToPath(string key)
      {
         var result = new List<object>();
         var matches = Regex.Matches(key, @"(?<prop>\w+)(?:\[(?<index>\d+)\]\.|\.)?");
         foreach (Match match in matches)
         {
            result.Add(match.Groups["prop"].Value);
            var index = match.Groups["index"];
            if (index.Success)
               result.Add(Convert.ToInt32(index.Value));
         }
         return result;
      }

      private Dictionary<string, object> ToProperties(ModelStateEntry entry)
      {
         if (entry.Errors.Count != 1)
            throw new InvalidOperationException("Can operate only on convertion errors. Count of errors is not right.");

         if (!(entry.Errors.Single().Exception is JsonReaderException exc))
            throw new InvalidOperationException("Can only operate on JsonReaderException");

         var type = Regex
            .Match(exc.Message, @"^Could not convert (?:\w+) to (?<type>\w+)")
            .Groups["type"];

         if (!type.Success)
            throw new InvalidOperationException("Can operate only on convertion errors. Format of error is not right.");

         return new Dictionary<string, object> { { "type", type.Value } };
      }

      private void Validate(ValidationAttribute attrib, Property property)
      {
         if (SpecialCases(attrib, property))
            return;

         if (!attrib.IsValid(property.Value))
            _errors.Add(new ValidationError(Map(attrib), property.Path, PropertiesExtractor.Extract(attrib)));
      }

      private string Map(ValidationAttribute attribute) =>
         attribute.GetType().Name.Replace("Attribute", "").ToCamelCase();

      private bool SpecialCases(ValidationAttribute attrib, Property property)
      {
         if (attrib is RequiredAttribute && property.Value is IEnumerable<object> list)
         {
            if (!list.Any())
               _errors.Add(new ValidationError("empty", property.Path));
            return true;
         }
         return false;
      }
   }
}
