using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class FailResponseTests
    {
        [Theory, AutoData]
        public void StatusIsFail(FailResponse response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be(JSendStatus.Fail);
        }

        [Theory, AutoData]
        public void ConstructorsThrowsWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (FailResponse).GetConstructors());
        }

        [Theory, AutoData]
        public void DataIsCorrectlyInitialized(string data)
        {
            // Exercise system
            var response = new FailResponse(data);
            // Verify outcome
            response.Data.Should().Be(data);
        }

        [Theory, AutoData]
        public void IsDeserializedCorrectly(string data)
        {
            // Fixture setup
            var serialized = new JObject
            {
                {"status", "fail"},
                {"data", data}
            };
            // Exercise system
            var deserialized = serialized.ToObject<FailResponse>();
            // Verify outcome
            deserialized.Data.Should().Be(data);
        }

        [Theory, AutoData]
        public void ToStringReturnsSerializedResponse(FailResponse response)
        {
            // Fixture setup
            var serialized = JsonConvert.SerializeObject(response,
                new JsonSerializerSettings {Formatting = Formatting.Indented});
            // Exercise system
            var responseAsString = response.ToString();
            // Verify outcome
            responseAsString.Should().Be(serialized);
        }
    }
}
