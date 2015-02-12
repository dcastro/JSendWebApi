using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JSendWebApi.Tests
{
    public class SuccessJSendResponseTests
    {
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
