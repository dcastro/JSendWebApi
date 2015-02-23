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
    public class FailResponseTests
    {

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
