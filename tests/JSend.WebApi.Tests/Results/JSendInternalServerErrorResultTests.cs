using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Xunit;

namespace JSend.WebApi.Tests.Results
{
    public class JSendInternalServerErrorResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendInternalServerErrorResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrownsWhenControllerIsNull(string message, int? errorCode, object data)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendInternalServerErrorResult(message, errorCode, data, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenMessageIsNull(int? errorCode, object data, ApiController controller)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendInternalServerErrorResult(null, errorCode, data, controller));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenMessageIsWhiteSpace(int? errorCode, object data, ApiController controller)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentException>(
                () => new JSendInternalServerErrorResult(" ", errorCode, data, controller));
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(string message, int? errorCode, Exception data, ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new ErrorResponse(message, errorCode, data);
            // Exercise system
            var result = new JSendInternalServerErrorResult(message, errorCode, data, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs500(JSendInternalServerErrorResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitializedUsingController(
            string message, int? code, Exception data, ApiController controller)
        {
            // Exercise system
            var result = new JSendInternalServerErrorResult(message, code, data, controller);
            // Verify outcome
            result.Request.Should().Be(controller.Request);
        }

        [Theory, JSendAutoData]
        public void MessageIsCorrectlyInitialized(string message, int? errorCode, Exception data,
            ApiController controller)
        {
            // Exercise system
            var result = new JSendInternalServerErrorResult(message, errorCode, data, controller);
            // Verify outcome
            result.Message.Should().Be(message);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendInternalServerErrorResult result)
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
        public async Task SetsStatusCode(JSendInternalServerErrorResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendInternalServerErrorResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
