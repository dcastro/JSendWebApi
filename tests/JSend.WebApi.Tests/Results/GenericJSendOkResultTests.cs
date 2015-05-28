using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using JSend.WebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.WebApi.Tests.Results
{
    public class GenericJSendOkResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendOkResult<Model> result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, null as ApiController));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenRequestIsNull(Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, null as HttpRequestMessage));
        }

        [Theory, JSendAutoData]
        public void CanBeCreatedWithControllerWithoutRequest(
            Model content, [NoAutoProperties] TestableJSendApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendOkResult<Model>(content, controller);
            ctor.ShouldNotThrow();
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(Model content, ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new SuccessResponse(content);
            // Exercise system
            var result = new JSendOkResult<Model>(content, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs200(JSendOkResult<Model> result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitialized(Model content, HttpRequestMessage request)
        {
            // Exercise system
            var result = new JSendOkResult<Model>(content, request);
            // Verify outcome
            result.Request.Should().Be(request);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitializedUsingController(Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendOkResult<Model>(content, controller);
            // Verify outcome
            result.Request.Should().Be(controller.Request);
        }

        [Theory, JSendAutoData]
        public void ContentIsCorrectlyInitialized(Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendOkResult<Model>(content, controller);
            // Verify outcome
            result.Content.Should().Be(content);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendOkResult<Model> result)
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
        public async Task SetsStatusCode(JSendOkResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendOkResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
