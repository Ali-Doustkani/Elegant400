using Elegant400.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Elegant400.Validation
{
   public static class PropertiesExtractor
   {
      public static Dictionary<string, object> Extract(ValidationAttribute attrib)
      {
         if (attrib is RequiredAttribute || attrib is EnumDataTypeAttribute)
            return Empty();

         if (attrib is CompareAttribute comp)
            return Pair("otherProperty", comp.OtherProperty);

         if (attrib is RangeAttribute range)
            return Pair("minimum", range.Minimum, "maximum", range.Maximum);

         if (attrib is RegularExpressionAttribute reg)
            return Pair("pattern", reg.Pattern);

         return Default(attrib);
      }

      private static Dictionary<string, object> Empty() => new Dictionary<string, object>();

      private static Dictionary<string, object> Pair(string prop, object val)
      {
         var ret = new Dictionary<string, object>();
         ret.Add(prop, val);
         return ret;
      }

      private static Dictionary<string, object> Pair(string prop1, object val1, string prop2, object val2)
      {
         var ret = new Dictionary<string, object>();
         ret.Add(prop1, val1);
         ret.Add(prop2, val2);
         return ret;
      }

      private static Dictionary<string, object> Default(ValidationAttribute attrib)
      {
         var result = new Dictionary<string, object>();
         foreach (var prop in OwnPropertiesOf(attrib))
         {
            if (prop.Name.Equals("error", StringComparison.CurrentCultureIgnoreCase) || prop.Name.Equals("path", StringComparison.CurrentCultureIgnoreCase))
               throw new InvalidOperationException("Error and Path are not valid names for properties.");
            result.Add(prop.Name.ToCamelCase(), prop.GetValue(attrib));
         }
         return result;
      }

      private static PropertyInfo[] OwnPropertiesOf(ValidationAttribute attrib) =>
         attrib.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
   }
}
