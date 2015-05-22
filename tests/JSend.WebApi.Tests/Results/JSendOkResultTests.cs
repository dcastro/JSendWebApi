using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Idioms;
using Xunit;

namespace JSend.WebApi.Tests.Results
{
    public class JSendOkResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendOkResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendOkResult).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new SuccessResponse();
            // Exercise system
            var result = new JSendOkResult(controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs200(JSendOkResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitialized(HttpRequestMessage request)
        {
            // Exercise system
            var result = new JSendOkResult(request);
            // Verify outcome
            result.Request.Should().Be(request);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitializedUsingController(ApiController controller)
        {
            // Exercise system
            var result = new JSendOkResult(controller);
            // Verify outcome
            result.Request.Should().Be(controller.Request);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendOkResult result)
        {
            // Fixture setup
            var serializedResponse = JsonConvert.SerializeObject(result.Response);
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            content.Should().Be(serializedResponse);
        }

        [Theory, JSendAutoData]
        public async Task SetsStatusCode(JSendOkResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendOkResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
