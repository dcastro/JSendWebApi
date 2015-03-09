using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class SuccessResponseTests
    {
        [Theory, AutoData]
        public void StatusIsSuccess(SuccessResponse<string> response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be(JSendStatus.Success);
        }

        [Theory, AutoData]
        public void DataIsCorrectlyInitialized(string data)
        {
            // Exercise system
            var response = new SuccessResponse<string>(data);
            // Verify outcome
            response.Data.Should().Be(data);
        }

        [Theory, AutoData]
        public void IsDeserializedCorrectly(string data)
        {
            // Fixture setup
            var serialized = new JObject
            {
                {"status", "success"},
                {"data", data}
            };
            // Exercise system
            var deserialized = serialized.ToObject<SuccessResponse<string>>();
            // Verify outcome
            deserialized.Data.Should().Be(data);
        }

        [Theory, AutoData]
        public void ToStringReturnsSerializedResponse(SuccessResponse<string> response)
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
