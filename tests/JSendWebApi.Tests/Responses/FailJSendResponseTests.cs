using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Tests.FixtureCustomizations;
using JSendWebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Responses
{
    public class FailJSendResponseTests
    {
        [Fact]
        public void ThrowsIfDataIsNull()
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new FailJSendResponse<Model>(null));
        }

        [Theory, JSendAutoData]
        public void StatusIsFail(FailJSendResponse<Model> response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be("fail");
        }

        [Theory, JSendAutoData]
        public void SerializesCorrectly([Frozen] Model model, FailJSendResponse<Model> response)
        {
            // Fixture setup
            var expectedSerializedResponse = new JObject
            {
                {"status", "fail"},
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
