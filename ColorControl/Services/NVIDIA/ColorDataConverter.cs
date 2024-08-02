using ColorControl.Shared.Contracts.NVIDIA;
using Newtonsoft.Json;
using NvAPIWrapper.Display;
using System;
using System.Collections.Generic;

namespace ColorControl.Services.NVIDIA
{
    public class ColorDataConverter : JsonConverter<ColorData>
    {
        public override bool CanWrite { get { return false; } }

        public override ColorData ReadJson(JsonReader reader, Type objectType, ColorData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = new Dictionary<string, object>();

            if (reader.TokenType == JsonToken.StartObject)
            {
                reader.Read();
                while (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = reader.Value;

                    reader.Read();
                    var value = reader.Value;

                    result.Add(propertyName.ToString(), value);

                    reader.Read();
                }
            }

            return NvPreset.GenerateColorData(result);
        }

        public override void WriteJson(JsonWriter writer, ColorData value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
