using System;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Tests.TestClasses;
using Newtonsoft.Json.Linq;
using Xunit;

namespace JSendWebApi.Tests.Responses
{
    public class GenericSuccessJSendResponseTests
    {
        [Fact]
        public void ThrowsIfDataIsNull()
        {
            // Fixture setup
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new SuccessJSendResponse<Model>(null));
        }

        [Fact]
        public void StatusIsSuccess()
        {
            // Fixture setup
            var model = new Model();
            var response = new SuccessJSendResponse<Model>(model);
            // Exercise system
            var status = response.Status;
            // Verify outcome
            status.Should().Be("success");
        }

        [Fact]
        public void SerializesCorrectly()
        {
            // Fixture setup
            var model = new Model();
            var response = new SuccessJSendResponse<Model>(model);

            var expectedSerializedResponse = new JObject
            {
                {"status", "success"},
                {"data", JObject.FromObject(model)}
            };
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            JToken.DeepEquals(serializedResponse, expectedSerializedResponse)
                .Should().BeTrue();
        }
    }
}
