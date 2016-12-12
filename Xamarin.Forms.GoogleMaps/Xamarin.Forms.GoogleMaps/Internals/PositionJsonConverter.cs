using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.Internals
{
    public class PositionJsonConverterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Position) || objectType == typeof(Position?);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var pos = (Position)value;

            writer.WriteStartObject();
            writer.WritePropertyName("Latitude");
            serializer.Serialize(writer, pos.Latitude);
            writer.WritePropertyName("Longitude");
            serializer.Serialize(writer, pos.Longitude);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var lat = default(double);
            var lng = default(double);

            var gotLatitude = false;
            var gotLongitude = false;
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    break;

                var propertyName = (string)reader.Value;
                if (!reader.Read())
                    continue;

                if (propertyName == "Latitude")
                {
                    lat = serializer.Deserialize<double>(reader);
                    gotLatitude = true;
                }

                if (propertyName == "Longitude")
                {
                    lng = serializer.Deserialize<double>(reader);
                    gotLongitude = true;
                }
            }

            // If only latitude or longitude: bad format. 
            // If no latitude and no longitude, empty object that should return default(Position)
            if ((gotLatitude && !gotLongitude)
                || (!gotLatitude && gotLongitude))
            {
                throw new InvalidDataException("A position must contain Latitude and Longitude properties.");
            }

            return new Position(lat, lng);
        }

    }
}
