using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Results;
using JSendWebApi.Tests.FixtureCustomizations;
using JSendWebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Results
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
        public void ConstructorThrowsWhenLocationIsNull(Model content, JSendApiController controller)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendCreatedResult<Model>(null, content, controller));
        }


        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(Uri location, Model content, JSendApiController controller)
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
        public void LocationIsCorrectlyInitialized(Uri location, Model content, JSendApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedResult<Model>(location, content, controller);
            // Verify outcome
            result.Location.Should().Be(location);
        }

        [Theory, JSendAutoData]
        public void ContentIsCorrectlyInitialized(Uri location, Model content, JSendApiController controller)
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
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendCreatedResult<Model>>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
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
