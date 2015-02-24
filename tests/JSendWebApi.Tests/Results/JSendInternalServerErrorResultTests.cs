using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Results
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
                () => new JSendInternalServerErrorResult(null, message, errorCode, data));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenMessageIsNull(JSendApiController controller, int? errorCode, object data)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendInternalServerErrorResult(controller, null, errorCode, data));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenMessageIsWhiteSpace(JSendApiController controller, int? errorCode, object data)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentException>(
                () => new JSendInternalServerErrorResult(controller, " ", errorCode, data));
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(JSendApiController controller, string message, int? errorCode,
            Exception data)
        {
            // Fixture setup
            var expectedResponse = new ErrorResponse(message, errorCode, data);
            // Exercise system
            var result = new JSendInternalServerErrorResult(controller, message, errorCode, data);
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
        public void MessageIsCorrectlyInitialized(JSendApiController controller, string message, int? errorCode,
            Exception data)
        {
            // Exercise system
            var result = new JSendInternalServerErrorResult(controller, message, errorCode, data);
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
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendInternalServerErrorResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
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
