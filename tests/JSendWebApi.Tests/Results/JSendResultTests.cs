using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Results;
using JSendWebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Results
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
        public void StatusCodeIsCorrectlyInitialized(JSendApiController controller, IJSendResponse response)
        {
            // Fixture setup
            const HttpStatusCode expectedStatusCode = HttpStatusCode.Continue;
            // Exercise system
            var result = new JSendResult<IJSendResponse>(controller, response, expectedStatusCode);
            // Verify outcome
            result.StatusCode.Should().Be(expectedStatusCode);
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized([Frozen] SuccessResponse response,
            JSendResult<SuccessResponse> result)
        {
            // Exercise system and verify outcome
            result.Response.Should().BeSameAs(response);
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
        public async Task SetsStatusCode(JSendApiController controller, SuccessResponse response)
        {
            // Fixture setup
            const HttpStatusCode expectedStatusCode = HttpStatusCode.InternalServerError;
            var result = new JSendResult<SuccessResponse>(controller, response, expectedStatusCode);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(expectedStatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendResult<SuccessResponse>>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendResult<SuccessResponse> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
