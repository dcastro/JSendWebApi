using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.FunctionalTests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JSend.WebApi.FunctionalTests
{
    public class ExceptionTests
    {
        [Theory, JSendAutoData]
        public async Task ActionReturnsStatusCode500(HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/exception");

                // Verify outcome
                response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            }
        }

        [Theory, JSendAutoData]
        public async Task ActionSetContentTypeHeader(HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/exception");

                // Verify outcome
                response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
                response.Content.Headers.ContentType.CharSet.Should().Be(Encoding.UTF8.WebName);
            }
        }

        [Theory, JSendAutoData]
        public async Task ActionReturnErrorResponse_WithExceptionDetails(HttpServer server, HttpClient client)
        {
            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/exception");

                // Fixture setup
                // the expected content has to be created *after* calling the controller
                // so that we can pick up on the exception's strack trace
                var expectedContent = new JObject
                {
                    {"status", "error"},
                    {"message", UsersController.TestException.Message},
                    {"data", UsersController.TestException.ToString()}
                };

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                Assert.Equal(expectedContent, content, JToken.EqualityComparer);
            }
        }

        [Theory, JSendAutoData]
        public async Task ActionReturnErrorResponse_WithoutExceptionDetails(HttpServer server, HttpClient client)
        {
            // Fixture setup
            var expectedContent = new JObject
            {
                {"status", "error"},
                {"message", "An error has occurred."}
            };

            using (server)
            using (client)
            {
                // Exercise system
                var response = await client.GetAsync("http://localhost/users/exception-without-details");

                // Verify outcome
                var content = JToken.Parse(await response.Content.ReadAsStringAsync());
                Assert.Equal(expectedContent, content, JToken.EqualityComparer);
            }
        }
    }
}
