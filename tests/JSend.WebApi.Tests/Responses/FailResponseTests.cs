using System;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests.Responses
{
    public class FailResponseTests
    {
        [Theory, JSendAutoData]
        public void IsJSendResponse(FailResponse response)
        {
            // Exercise system and verify outcome
            response.Should().BeAssignableTo<IJSendResponse>();
        }

        [Theory, JSendAutoData]
        public void StatusIsFail(FailResponse response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be("fail");
        }

        [Fact]
        public void ConstructorThrowsWhenDataIsNull()
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new FailResponse(null));
        }

        [Theory, JSendAutoData]
        public void DataIsCorrectlyInitialized(object data)
        {
            // Exercise system
            var response = new FailResponse(data);
            // Verify outcome
            response.Data.Should().BeSameAs(data);
        }

        [Theory, JSendAutoData]
        public void SerializesCorrectly(object data, FailResponse response)
        {
            // Fixture setup
            var expectedSerializedResponse = new JObject
            {
                {"status", "fail"},
                {"data", JObject.FromObject(data)}
            };
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            JToken.DeepEquals(serializedResponse, expectedSerializedResponse)
                .Should().BeTrue();
        }
    }
}
