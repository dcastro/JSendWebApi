using Newtonsoft.Json;

namespace JSend.WebApi.Tests.TestTypes
{
    public class Model
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
