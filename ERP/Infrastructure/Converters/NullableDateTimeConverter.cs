using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ERP.Infrastructure.Converters
{
    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return null;
                }
                
                if (DateTime.TryParse(stringValue, out DateTime date))
                {
                    return date;
                }
            }
            
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            // Fallback for unexpected formats
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("O")); // ISO 8601
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}

