using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiAutomationFramework
{

    public static class SerializationHelper
    {
        /// <summary>
        /// Converts an object to JSON string.
        /// </summary>
        public static string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Converts JSON string to strongly typed object.
        /// Throws exception if deserialization fails or returns null.
        /// </summary>
        public static T DeserializeObject<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("JSON string cannot be null or empty.");
            }

            var result = JsonConvert.DeserializeObject<T>(json);

            if (result == null)
            {
                throw new InvalidOperationException($"Deserialization failed for type {typeof(T).Name}. JSON: {json}");
            }

            return result;
        }

    }
}
