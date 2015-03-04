using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using JSend.WebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Xunit;
using Xunit.Extensions;

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
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenFormatterIsNull(Model model, HttpRequestMessage request)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, null, request));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenRequestIsNull(Model model, JsonMediaTypeFormatter formatter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, formatter, null));
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
