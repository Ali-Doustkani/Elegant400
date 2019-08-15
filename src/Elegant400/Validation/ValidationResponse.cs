using System.Collections.Generic;

namespace Elegant400.Validation
{
   /// <summary>
   /// The object model of Elegant400 response object.
   /// </summary>
   public class ValidationResponse
   {
      public ValidationResponse(string title, IEnumerable<ValidationError> errors)
      {
         Title = title;
         Errors = errors;
      }

      public string Title { get; }
      public IEnumerable<ValidationError> Errors { get; }
   }
}
