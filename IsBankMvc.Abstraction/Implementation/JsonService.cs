using IsBankMvc.Abstraction.Contracts;
using System.Text.Json;

namespace IsBankMvc.Abstraction.Implementation
{
    internal class JsonService : IJsonService
    {
        public T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
