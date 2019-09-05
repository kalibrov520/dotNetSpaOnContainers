using System;
using System.IO;
using Newtonsoft.Json;

namespace ApiGw_Base.Utils
{
    public class JsonLoader
    {
        public static T LoadFromFile<T>(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var json = reader.ReadToEnd();
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
        }

        public static T Deserialize<T>(object jsonObject) =>
            JsonConvert.DeserializeObject<T>(Convert.ToString(jsonObject));

    }
}