using Elegant400.Validation;
using Newtonsoft.Json;
using System;

namespace Elegant400.Utils
{
   public class ValidationResponseConverter : JsonConverter
   {
      public override bool CanConvert(Type objectType) =>
         objectType == typeof(ValidationResponse);

      public override bool CanRead => false;

      public override bool CanWrite => true;

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         throw new NotImplementedException();
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         if (value == null)
         {
            serializer.Serialize(writer, null);
            return;
         }

         var response = (ValidationResponse)value;
         writer.WriteStartObject();
         writer.WritePropertyName("title");
         writer.WriteValue(response.Title);
         writer.WritePropertyName("errors");
         writer.WriteStartArray();
         foreach (var err in response.Errors)
            WriteError(writer, err);
         writer.WriteEndArray();
         writer.WriteEndObject();
      }

      private void WriteError(JsonWriter writer, ValidationError err)
      {
         writer.WriteStartObject();
         writer.WritePropertyName("error");
         writer.WriteValue(err.Error);

         writer.WritePropertyName("path");
         writer.WriteStartArray();
         foreach (var path in err.Path)
            writer.WriteValue(path);
         writer.WriteEndArray();

         foreach (var prop in err.Properties)
         {
            writer.WritePropertyName(prop.Key);
            writer.WriteValue(prop.Value);
         }
         writer.WriteEndObject();
      }
   }
}
