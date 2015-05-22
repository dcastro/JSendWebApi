using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using Xunit;

namespace JSend.WebApi.Tests.Results
{
    public class JSendUnauthorizedResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendUnauthorizedResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendUnauthorizedResult).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(IEnumerable<AuthenticationHeaderValue> challenges,
            ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new FailResponse(StringResources.RequestNotAuthorized);
            // Exercise system
            var result = new JSendUnauthorizedResult(challenges, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs401(JSendUnauthorizedResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitialized(
            IEnumerable<AuthenticationHeaderValue> challenges, HttpRequestMessage request)
        {
            // Exercise system
            var result = new JSendUnauthorizedResult(challenges, request);
            // Verify outcome
            result.Request.Should().Be(request);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitializedUsingController(
            IEnumerable<AuthenticationHeaderValue> challenges, ApiController controller)
        {
            // Exercise system
            var result = new JSendUnauthorizedResult(challenges, controller);
            // Verify outcome
            result.Request.Should().Be(controller.Request);
        }

        [Theory, JSendAutoData]
        public void ChallengesAreCorrectlyInitialized(List<AuthenticationHeaderValue> challenges,
            ApiController controller)
        {
            // Exercise system
            var result = new JSendUnauthorizedResult(challenges, controller);
            // Verify outcome
            result.Challenges.Should().BeEquivalentTo(challenges);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendUnauthorizedResult result)
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
        public async Task SetsStatusCode(JSendUnauthorizedResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendUnauthorizedResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task SetsLocationHeader(JSendUnauthorizedResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Headers.WwwAuthenticate.Should().BeEquivalentTo(result.Challenges);
        }
    }
}
