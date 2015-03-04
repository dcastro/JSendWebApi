using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace JSend.WebApi.Tests.Results
{
    public class JSendResultTests
    {
        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendResult<SuccessResponse>).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(HttpStatusCode code, IJSendResponse response,
            JSendApiController controller)
        {
            // Exercise system
            var result = new JSendResult<IJSendResponse>(code, response, controller);
            // Verify outcome
            result.Response.Should().BeSameAs(response);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIsCorrectlyInitialized(HttpStatusCode expectedStatusCode, IJSendResponse response,
            JSendApiController controller)
        {
            // Exercise system
            var result = new JSendResult<IJSendResponse>(expectedStatusCode, response, controller);
            // Verify outcome
            result.StatusCode.Should().Be(expectedStatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SerializesResponse(JSendResult<SuccessResponse> result)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(result.Response);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task SetsStatusCode(JSendResult<IJSendResponse> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendResult<IJSendResponse> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
