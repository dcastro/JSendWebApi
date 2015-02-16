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
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Responses
{
    public class ErrorJSendResponseTests
    {
        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenMessageIsNull(int code, object data)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new ErrorJSendResponse(null));
            Assert.Throws<ArgumentNullException>(() => new ErrorJSendResponse(null, code));
            Assert.Throws<ArgumentNullException>(() => new ErrorJSendResponse(null, data));
            Assert.Throws<ArgumentNullException>(() => new ErrorJSendResponse(null, code, data));
        }

        [Theory, JSendAutoData]
        public void StatusIsError(ErrorJSendResponse response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be("error");
        }

        [Theory, JSendAutoData]
        public void MessageIsCorrectlyInitialized(string message)
        {
            // Exercise system
            var response = new ErrorJSendResponse(message);
            // Verify outcome
            response.Message.Should().Be(message);
        }

        [Theory, JSendAutoData]
        public void MessageAndCodeAreCorrectlyInitialized(string message, int code)
        {
            // Exercise system
            var response = new ErrorJSendResponse(message, code);
            // Verify outcome
            response.Message.Should().Be(message);
            response.Code.Should().HaveValue();
            response.Code.Should().Be(code);
        }

        [Theory, JSendAutoData]
        public void MessageAndDataAreCorrectlyInitialized(string message, object data)
        {
            // Exercise system
            var response = new ErrorJSendResponse(message, data);
            // Verify outcome
            response.Message.Should().Be(message);
            response.Data.Should().BeSameAs(data);
        }

        [Theory, JSendAutoData]
        public void MessageCodeAndDataAreCorrectlyInitialized(string message, int code, object data)
        {
            // Exercise system
            var response = new ErrorJSendResponse(message, code, data);
            // Verify outcome
            response.Message.Should().Be(message);
            response.Code.Should().HaveValue();
            response.Code.Should().Be(code);
            response.Data.Should().BeSameAs(data);
        }

        [Theory, JSendAutoData]
        public void CodeIsNullByDefault(string message)
        {
            // Exercise system
            var response = new ErrorJSendResponse(message);
            // Verify outcome
            response.Code.Should().NotHaveValue();
        }

        [Theory, JSendAutoData]
        public void DataIsNullByDefault(string message)
        {
            // Exercise system
            var response = new ErrorJSendResponse(message);
            // Verify outcome
            response.Data.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public void SerializesCorrectly(string message, int code, Model data)
        {
            // Fixture setup
            var expectedSerializedResponse = new JObject
            {
                {"status", "error"},
                {"message", message},
                {"code", code},
                {"data", JObject.FromObject(data)}
            };

            var response = new ErrorJSendResponse(message, code, data);
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            JToken.DeepEquals(expectedSerializedResponse, serializedResponse)
                .Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void NullCodeIsNotSerialized(string message)
        {
            // Fixture setup
            var response = new ErrorJSendResponse(message);
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            serializedResponse.Should().NotContainKey("code");
        }

        [Theory, JSendAutoData]
        public void NullDataIsNotSerialized(string message)
        {
            // Fixture setup
            var response = new ErrorJSendResponse(message);
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            serializedResponse.Should().NotContainKey("data");
        }
    }
}
