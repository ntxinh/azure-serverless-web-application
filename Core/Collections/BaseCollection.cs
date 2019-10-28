using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Core.Collections
{
    public abstract class BaseCollection
    {
        [Key]
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
