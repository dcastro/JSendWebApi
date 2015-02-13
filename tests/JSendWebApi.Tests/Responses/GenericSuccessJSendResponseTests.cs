using System;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Tests.FixtureCustomizations;
using JSendWebApi.Tests.TestTypes;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Responses
{
    public class GenericSuccessJSendResponseTests
    {
        [Fact]
        public void ThrowsIfDataIsNull()
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new SuccessJSendResponse<Model>(null));
        }

        [Theory, JSendAutoData]
        public void StatusIsSuccess(SuccessJSendResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be("success");
        }

        [Theory, JSendAutoData]
        public void DataIsCorrectlyInitialized(Model model)
        {
            // Exercise system
            var response = new SuccessJSendResponse<Model>(model);
            // Verify outcome
            response.Data.Should().BeSameAs(model);
        }

        [Theory, JSendAutoData]
        public void SerializesCorrectly([Frozen] Model model, SuccessJSendResponse<Model> response)
        {
            // Fixture setup
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
