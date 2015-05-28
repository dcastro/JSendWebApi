using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

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
        public void CanBeCreatedWithControllerWithoutRequest(HttpStatusCode status, IJSendResponse response,
            [NoAutoProperties] TestableJSendApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendResult<IJSendResponse>(status, response, controller);
            ctor.ShouldNotThrow();
        }

        [Theory, JSendAutoData]
        public void ExecuteThrowsWhenControllerHasNoRequest(HttpStatusCode status, IJSendResponse response,
            [NoAutoProperties] TestableJSendApiController controller)
        {
            // Fixture setup
            var result = new JSendResult<IJSendResponse>(status, response, controller);
            // Exercise system and verify outcome
            result.Awaiting(r => r.ExecuteAsync(CancellationToken.None))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("ApiController.Request must not be null.");
        }

        [Theory, JSendAutoData]
        public void ExecuteThrowsWhenRequestHasNoContext(HttpStatusCode status, IJSendResponse response)
        {
            // Fixture setup
            var request = new HttpRequestMessage();
            var result = new JSendResult<IJSendResponse>(status, response, request);
            // Exercise system and verify outcome
            result.Awaiting(r => r.ExecuteAsync(CancellationToken.None))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(StringResources.Request_RequestContextMustNotBeNull);
        }

        [Theory, JSendAutoData]
        public void ExecuteThrowsWhenRequestContextHasNoConfiguration(HttpRequestMessage request, HttpStatusCode status,
            IJSendResponse response)
        {
            // Fixture setup
            var requestContext = new HttpRequestContext();
            request.SetRequestContext(requestContext);

            var result = new JSendResult<IJSendResponse>(status, response, request);
            // Exercise system and verify outcome
            result.Awaiting(r => r.ExecuteAsync(CancellationToken.None))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage("HttpRequestContext.Configuration must not be null.");
        }

        [Theory, JSendAutoData]
        public void ExecuteThrowsWhenControllerHasNoJsonFormatter(HttpStatusCode status, IJSendResponse response,
            ApiController controller)
        {
            // Fixture setup
            var formatters = controller.Configuration.Formatters;
            formatters.OfType<JsonMediaTypeFormatter>().ToList()
                .ForEach(f => formatters.Remove(f));

            var result = new JSendResult<IJSendResponse>(status, response, controller);
            // Exercise system and verify outcome
            result.Awaiting(r => r.ExecuteAsync(CancellationToken.None))
                .ShouldThrow<InvalidOperationException>()
                .WithMessage(StringResources.ConfigurationMustContainFormatter);
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(HttpStatusCode code, IJSendResponse response,
            ApiController controller)
        {
            // Exercise system
            var result = new JSendResult<IJSendResponse>(code, response, controller);
            // Verify outcome
            result.Response.Should().BeSameAs(response);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIsCorrectlyInitialized(HttpStatusCode expectedStatusCode, IJSendResponse response,
            ApiController controller)
        {
            // Exercise system
            var result = new JSendResult<IJSendResponse>(expectedStatusCode, response, controller);
            // Verify outcome
            result.StatusCode.Should().Be(expectedStatusCode);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitialized(HttpStatusCode status, IJSendResponse response, HttpRequestMessage request)
        {
            // Exercise system
            var result = new JSendResult<IJSendResponse>(status, response, request);
            // Verify outcome
            result.Request.Should().Be(request);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitializedUsingController(HttpStatusCode status, IJSendResponse response, ApiController controller)
        {
            // Exercise system
            var result = new JSendResult<IJSendResponse>(status, response, controller);
            // Verify outcome
            result.Request.Should().Be(controller.Request);
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
