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
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace JSend.WebApi.Tests.Results
{
    public class JSendBadRequestResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendBadRequestResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendBadRequestResult).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenReasonIsWhiteSpace(JSendApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendBadRequestResult("  ", controller);
            ctor.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain(StringResources.BadRequest_WhiteSpaceReason);
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(string reason, JSendApiController controller)
        {
            // Fixture setup
            var expectedResponse = new FailResponse(reason);
            // Exercise system
            var result = new JSendBadRequestResult(reason, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs400(JSendBadRequestResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory, JSendAutoData]
        public void ReasonIsCorrectlyInitialized(string reason, JSendApiController controller)
        {
            // Exercise system
            var result = new JSendBadRequestResult(reason, controller);
            // Verify outcome
            result.Reason.Should().Be(reason);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendBadRequestResult result)
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
        public async Task SetsStatusCode(JSendBadRequestResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendBadRequestResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
