using System;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using JSend.WebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests.Responses
{
    public class SuccessResponseTests
    {
        [Theory, JSendAutoData]
        public void IsJSendResponse(SuccessResponse response)
        {
            // Exercise system and verify outcome
            response.Should().BeAssignableTo<IJSendResponse>();
        }

        [Theory, JSendAutoData]
        public void StatusIsSuccess(SuccessResponse response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be("success");
        }

        [Theory, JSendAutoData]
        public void DataIsCorrectlyInitialized(Model model)
        {
            // Exercise system
            var response = new SuccessResponse(model);
            // Verify outcome
            response.Data.Should().BeSameAs(model);
        }

        [Fact]
        public void DataIsNullByDefault()
        {
            // Exercise system
            var response = new SuccessResponse();
            // Verify outcome
            response.Data.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public void SerializesCorrectly(object data, [Greedy] SuccessResponse response)
        {
            // Fixture setup
            var expectedSerializedResponse = new JObject
            {
                {"status", "success"},
                {"data", JObject.FromObject(data)}
            };

            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            JToken.DeepEquals(serializedResponse, expectedSerializedResponse)
                .Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void NullDataIsSerialized()
        {
            // Fixture setup
            var response = new SuccessResponse();
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            serializedResponse.Should().ContainKey("data");
            serializedResponse.Value<string>("data").Should().BeNull();
        }
    }
}
