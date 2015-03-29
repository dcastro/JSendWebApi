using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.FunctionalTests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Xunit.Extensions;

namespace JSend.WebApi.FunctionalTests
{
    public class HttpActionResultTests
    {
        [Theory, JSendAutoData]
        public async Task Ok_Returns_ExpectedResponse(HttpServer server, HttpClient client)
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
                var response = await client.GetAsync("http://localhost/users/ok");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();
            }
        }

        [Theory, JSendAutoData]
        public async Task OkWithData_Returns_ExpectedResponse(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "success"},
                {"data", JObject.FromObject(UsersController.TestUser)}
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/ok-with-user");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();
            }
        }

        [Theory, JSendAutoData]
        public async Task CreatedWithString_Returns_ExpectedResponse(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "success"},
                {"data", JObject.FromObject(UsersController.TestUser)}
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/created-with-string");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();
                response.Headers.Location.Should().Be(UsersController.CreatedLocation);
            }
        }

        [Theory, JSendAutoData]
        public async Task CreatedWithUri_Returns_ExpectedResponse(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "success"},
                {"data", JObject.FromObject(UsersController.TestUser)}
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/created-with-uri");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();
                response.Headers.Location.Should().Be(UsersController.CreatedLocation);
            }
        }

        [Theory, JSendAutoData]
        public async Task CreatedAtRouteWithObject_Returns_ExpectedResponse(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "success"},
                {"data", JObject.FromObject(UsersController.TestUser)}
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/created-at-route-with-object");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();
                response.Headers.Location.Should().Be(UsersController.CreatedLocation);
            }
        }

        [Theory, JSendAutoData]
        public async Task CreatedAtRouteWithDictionary_Returns_ExpectedResponse(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "success"},
                {"data", JObject.FromObject(UsersController.TestUser)}
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/created-at-route-with-dictionary");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();
                response.Headers.Location.Should().Be(UsersController.CreatedLocation);
            }
        }
    }
}
