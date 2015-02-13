using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Responses
{
    public class SuccessJSendResponseTests
    {
        [Theory, JSendAutoData]
        public void StatusIsSuccess(SuccessJSendResponse response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be("success");
        }

        [Theory, JSendAutoData]
        public void SerializesCorrectly(SuccessJSendResponse response)
        {
            // Fixture setup
            var expectedSerializedResponse = JObject.Parse(@"{""status"":""success""}");
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            JToken.DeepEquals(serializedResponse, expectedSerializedResponse)
                .Should().BeTrue();
        }
    }
}
