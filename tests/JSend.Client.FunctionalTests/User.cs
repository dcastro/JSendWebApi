using Newtonsoft.Json;

namespace JSend.Client.FunctionalTests
{
    public class User
    {
        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
