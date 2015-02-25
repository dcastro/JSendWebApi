using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Properties;
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
            Assert.Throws<ArgumentNullException>(() => new JSendNotFoundResult(reason, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenReasonIsWhiteSpace(JSendApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendNotFoundResult("  ", controller);
            ctor.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain(StringResources.NotFound_WhiteSpaceReason);
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(JSendApiController controller, string reason)
        {
            // Fixture setup
            var expectedResponse = new FailResponse(reason);
            // Exercise system
            var result = new JSendNotFoundResult(reason, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs404(JSendNotFoundResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory, JSendAutoData]
        public void ReasonIsCorrectlyInitialized(JSendApiController controller, string reason)
        {
            // Exercise system
            var result = new JSendNotFoundResult(reason, controller);
            // Verify outcome
            result.Reason.Should().Be(reason);
        }

        [Theory, JSendAutoData]
        public void ReasonIsSetToDefaultMessage_When_ArgumentIsNull(JSendApiController controller)
        {
            // Exercise system
            var result = new JSendNotFoundResult(null, controller);
            // Verify outcome
            result.Reason.Should().Be(StringResources.NotFound_DefaultMessage);
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
        public async Task SetsStatusCode(JSendNotFoundResult result)
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
