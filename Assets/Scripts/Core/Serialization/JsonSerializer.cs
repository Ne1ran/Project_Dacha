using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Core.Serialization
{
    [UsedImplicitly]
    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(string serialized)
        {
            return JsonConvert.DeserializeObject<T>(serialized)!;
        }

        public string Serialize<T>(T serialized)
        {
            return JsonConvert.SerializeObject(serialized)!;
        }
    }
}