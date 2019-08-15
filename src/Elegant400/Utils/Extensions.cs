namespace Elegant400.Utils
{
   public static class Extensions
   {
      public static string ToCamelCase(this string value) =>
         char.ToLowerInvariant(value[0]) + value.Substring(1);
   }
}
