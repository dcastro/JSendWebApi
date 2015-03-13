using Newtonsoft.Json;

namespace JSend.Client.Tests.TestTypes
{
    public class Model
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
