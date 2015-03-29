using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.FunctionalTests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.FunctionalTests
{
    public class VoidActionTests
    {
        [Theory, JSendAutoData]
        public async Task ActionReturnsStatusCode200(HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/void");

                // Verify outcome
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Theory, JSendAutoData]
        public async Task ActionSetContentTypeHeader(HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/void");

                // Verify outcome
                response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
                response.Content.Headers.ContentType.CharSet.Should().Be(Encoding.UTF8.WebName);
            }
        }

        [Theory, JSendAutoData]
        public async Task ActionReturnSuccessResponse(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "success"},
                {"data", null}
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/void");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                Assert.Equal(expectedContent, content, JToken.EqualityComparer);
            }
        }
    }
}
