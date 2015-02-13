using Newtonsoft.Json;

namespace JSendWebApi.Tests.TestTypes
{
    public class Model
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
