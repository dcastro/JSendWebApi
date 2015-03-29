using Newtonsoft.Json;

namespace JSend.WebApi.FunctionalTests
{
    public class User
    {
        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
