using System.Collections.Generic;
using System.Linq;

namespace Elegant400.Validation
{
   /// <summary>
   /// The object model of Elegant400 error
   /// </summary>
   public class ValidationError
   {
      public ValidationError(string error, IEnumerable<object> path)
      {
         Error = error;
         Path = path;
         Properties = new Dictionary<string, object>();
      }

      public ValidationError(string error, IEnumerable<object> path, Dictionary<string, object> properties)
         : this(error, path)
      {
         Properties = properties;
      }

      public string Error { get; }

      public IEnumerable<object> Path { get; }

      public Dictionary<string, object> Properties { get; }
   }
}
