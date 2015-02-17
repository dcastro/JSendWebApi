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
    public class JSendNotFoundResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendNotFoundResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(string reason)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendNotFoundResult(null, reason));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenReasonIsWhiteSpace(JSendApiController controller)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentException>(() => new JSendNotFoundResult(controller, "  "));
        }

        [Theory, JSendAutoData]
        public void ResponseIsInitialized(JSendNotFoundResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public void ResponseIsFail(JSendNotFoundResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().BeAssignableTo<FailJSendResponse>();
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendNotFoundResult result)
        {
            // Fixture setup
            var serializedResponse = JsonConvert.SerializeObject(result.Response);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(serializedResponse);
        }

        [Theory, JSendAutoData]
        public void ResponseDataIsCorrectlySet([Frozen] string reason, JSendNotFoundResult result)
        {
            // Exercise system and verify outcome
            result.Response.Data.Should().Be(reason);
        }

        [Theory, JSendAutoData]
        public void ResponseDataIsSetToDefaultMessage_When_ReasonIsNull(JSendApiController controller)
        {
            // Exercise system
            var result = new JSendNotFoundResult(controller, null);
            // Verify outcome
            result.Response.Data.Should().Be("The requested resource could not be found.");
        }

        [Theory, JSendAutoData]
        public async Task StatusCodeIs404(JSendNotFoundResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory, JSendAutoData]
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendNotFoundResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendNotFoundResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
