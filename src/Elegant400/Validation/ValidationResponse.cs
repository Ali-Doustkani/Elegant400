using Elegant400.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elegant400.Validation
{
   /// <summary>
   /// The object model of Elegant400 response object.
   /// </summary>
   [JsonConverter(typeof(ValidationResponseConverter))]
   public class ValidationResponse
   {
      public ValidationResponse(string title, IEnumerable<ValidationError> errors)
      {
         if (!errors.Any())
            throw new ArgumentException("errors should contain one element at least");

         Title = title;
         Errors = errors;
      }

      public string Title { get; }
      public IEnumerable<ValidationError> Errors { get; }
   }
}
