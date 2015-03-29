using System.Net;
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

                response.StatusCode.Should().Be(HttpStatusCode.OK);
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

                response.StatusCode.Should().Be(HttpStatusCode.OK);
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

                response.StatusCode.Should().Be(HttpStatusCode.Created);
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

                response.StatusCode.Should().Be(HttpStatusCode.Created);
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

                response.StatusCode.Should().Be(HttpStatusCode.Created);
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

                response.StatusCode.Should().Be(HttpStatusCode.Created);
            }
        }

        [Theory, JSendAutoData]
        public async Task RedirectWithString_Returns_ExpectedResponse(HttpServer server, HttpClient client)
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
                var response = await client.GetAsync("http://localhost/users/redirect-with-string");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();

                response.Headers.Location.Should().Be(UsersController.CreatedLocation);

                response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            }
        }

        [Theory, JSendAutoData]
        public async Task RedirectWithUri_Returns_ExpectedResponse(HttpServer server, HttpClient client)
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
                var response = await client.GetAsync("http://localhost/users/redirect-with-uri");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();

                response.Headers.Location.Should().Be(UsersController.CreatedLocation);

                response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            }
        }

        [Theory, JSendAutoData]
        public async Task RedirectToRouteWithObject_Returns_ExpectedResponse(HttpServer server, HttpClient client)
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
                var response = await client.GetAsync("http://localhost/users/redirect-to-route-with-object");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();

                response.Headers.Location.Should().Be(UsersController.CreatedLocation);

                response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            }
        }

        [Theory, JSendAutoData]
        public async Task RedirectToRouteWithDictionary_Returns_ExpectedResponse(HttpServer server, HttpClient client)
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
                var response = await client.GetAsync("http://localhost/users/redirect-to-route-with-dictionary");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();

                response.Headers.Location.Should().Be(UsersController.CreatedLocation);

                response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            }
        }

        [Theory, JSendAutoData]
        public async Task BadRequestWithReason_Returns_ExpectedResponse(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "fail"},
                {"data", UsersController.ErrorMessage}
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/badrequest-with-reason");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Theory, JSendAutoData]
        public async Task BadRequestWithModelState_Returns_ExpectedResponse(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "fail"},
                {
                    "data", new JObject
                    {
                        {UsersController.ModelErrorKey, new JArray(UsersController.ModelErrorValue)}
                    }
                }
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/badrequest-with-modelstate");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                JToken.DeepEquals(expectedContent, content).Should().BeTrue();

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }
    }
}
