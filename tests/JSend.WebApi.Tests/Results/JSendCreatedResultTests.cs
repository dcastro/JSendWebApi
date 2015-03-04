using System;
using System.Net;
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
    public class JSendCreatedResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendCreatedResult<Model> result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(Uri location, Model content)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendCreatedResult<Model>(location, content, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenLocationIsNull(Model content, ApiController controller)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendCreatedResult<Model>(null, content, controller));
        }


        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(Uri location, Model content, ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new SuccessResponse(content);
            // Exercise system
            var result = new JSendCreatedResult<Model>(location, content, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs201(JSendCreatedResult<Model> result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory, JSendAutoData]
        public void LocationIsCorrectlyInitialized(Uri location, Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedResult<Model>(location, content, controller);
            // Verify outcome
            result.Location.Should().Be(location);
        }

        [Theory, JSendAutoData]
        public void ContentIsCorrectlyInitialized(Uri location, Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedResult<Model>(location, content, controller);
            // Verify outcome
            result.Content.Should().Be(content);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendCreatedResult<Model> result)
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
        public async Task SetsStatusCode(JSendCreatedResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendCreatedResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task SetsLocationHeader(JSendCreatedResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Headers.Location.Should().Be(result.Location);
        }
    }
}
