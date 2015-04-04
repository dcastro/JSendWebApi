using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests.Results
{
    public class JSendNotFoundResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendNotFoundResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(string reason)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendNotFoundResult(reason, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenReasonIsWhiteSpace(ApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendNotFoundResult("  ", controller);
            ctor.ShouldThrow<ArgumentException>()
                .And.Message.Should().StartWith(StringResources.NotFound_WhiteSpaceReason);
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(string reason, ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new FailResponse(reason);
            // Exercise system
            var result = new JSendNotFoundResult(reason, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs404(JSendNotFoundResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory, JSendAutoData]
        public void ReasonIsCorrectlyInitialized(string reason, ApiController controller)
        {
            // Exercise system
            var result = new JSendNotFoundResult(reason, controller);
            // Verify outcome
            result.Reason.Should().Be(reason);
        }

        [Theory, JSendAutoData]
        public void ReasonIsSetToDefaultMessage_When_ArgumentIsNull(ApiController controller)
        {
            // Exercise system
            var result = new JSendNotFoundResult(null, controller);
            // Verify outcome
            result.Reason.Should().Be(StringResources.NotFound_DefaultMessage);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendNotFoundResult result)
        {
            // Fixture setup
            var serializedResponse = JsonConvert.SerializeObject(result.Response);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(serializedResponse);
        }

        [Theory, JSendAutoData]
        public async Task SetsStatusCode(JSendNotFoundResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendNotFoundResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
