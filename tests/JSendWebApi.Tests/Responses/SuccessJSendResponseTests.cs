using FluentAssertions;
using JSendWebApi.Responses;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JSendWebApi.Tests.Responses
{
    public class SuccessJSendResponseTests
    {
        [Fact]
        public void StatusIsSuccess()
        {
            // Fixture setup
            var response = new SuccessJSendResponse();
            // Exercise system
            var status = response.Status;
            // Verify outcome
            status.Should().Be("success");
        }

        [Fact]
        public void SerializesCorrectly()
        {
            // Fixture setup
            var response = new SuccessJSendResponse();
            var expectedSerializedResponse = JObject.Parse(@"{""status"":""success""}");
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            JToken.DeepEquals(serializedResponse, expectedSerializedResponse)
                .Should().BeTrue();
        }
    }
}
