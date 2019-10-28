using Newtonsoft.Json;

namespace Core.Collections
{
    public class Template : BaseCollection
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}
