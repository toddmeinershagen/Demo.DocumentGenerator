using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Demo.DocumentGenerator.Core.UnitTests.Converters
{
    public class ObjectDictionaryConverter : CustomCreationConverter<IDictionary<string, object>>
    {
        public override IDictionary<string, object> Create(Type objectType)
        {
            return new Dictionary<string, object>();
        }

        /// <summary>
        /// What object types do I support?
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        /// <remarks>
        /// In addition to handling IDictionary&lt;string, object&gt;, we want to handle the deserialization of the individual dictionary values as well which are of type object.
        /// </remarks>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object) || base.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Null)
                return base.ReadJson(reader, objectType, existingValue, serializer);

            return reader.TokenType == JsonToken.StartArray ? 
                serializer.Deserialize<List<dynamic>>(reader) : 
                serializer.Deserialize(reader);
        }
    }
}
