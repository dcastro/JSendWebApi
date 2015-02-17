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
        public void ResponseIsInitialized(JSendInternalServerErrorResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public void ResponseIsError(JSendInternalServerErrorResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().BeAssignableTo<ErrorJSendResponse>();
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
        public void ResponseMessageIsCorrectlySet([Frozen] string message, JSendInternalServerErrorResult result)
        {
            // Exercise system and verify outcome
            result.Response.Message.Should().Be(message);
        }

        [Theory, JSendAutoData]
        public void ResponseCodeIsCorrectlySet([Frozen] int? code, JSendInternalServerErrorResult result)
        {
            // Exercise system and verify outcome
            result.Response.Code.Should().HaveValue()
                .And.Be(code);
        }

        [Theory, JSendAutoData]
        public void ResponseDataIsCorrectlySet([Frozen] object data, JSendInternalServerErrorResult result)
        {
            // Exercise system and verify outcome
            result.Response.Data.Should().BeSameAs(data);
        }

        [Theory, JSendAutoData]
        public async Task StatusCodeIs500(JSendInternalServerErrorResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
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
