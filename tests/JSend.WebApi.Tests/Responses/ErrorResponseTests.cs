using System;
using System.Collections.Generic;
using FluentAssertions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using JSend.WebApi.Tests.TestTypes;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture.Idioms;
using Xunit;

namespace JSend.WebApi.Tests.Responses
{
    public class ErrorResponseTests
    {
        [Theory, JSendAutoData]
        public void IsJSendResponse(ErrorResponse response)
        {
            // Exercise system and verify outcome
            response.Should().BeAssignableTo<IJSendResponse>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenMessageIsNull(int code, object data)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new ErrorResponse(null));
            Assert.Throws<ArgumentNullException>(() => new ErrorResponse(null, code));
            Assert.Throws<ArgumentNullException>(() => new ErrorResponse(null, data));
            Assert.Throws<ArgumentNullException>(() => new ErrorResponse(null, code, data));
        }

        public static IEnumerable<object[]> ConstructorCallsWithWhiteSpaceMessage
        {
            get
            {
                yield return new Action[] {() => new ErrorResponse(" ")};
                yield return new Action[] {() => new ErrorResponse(" ", 2)};
                yield return new Action[] {() => new ErrorResponse(" ", 2, "data")};
                yield return new Action[] {() => new ErrorResponse(" ", "data")};
            }
        }

        [Theory]
        [MemberData("ConstructorCallsWithWhiteSpaceMessage")]
        public void ConstructorsThrowWhenMessageIsWhiteSpace(Action ctor)
        {
            ctor.ShouldThrow<ArgumentException>()
                .And.Message.Should().StartWith(StringResources.ErrorResponse_WhiteSpaceMessage);
        }

        [Theory, JSendAutoData]
        public void StatusIsError(ErrorResponse response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be("error");
        }

        [Theory]
        [InlineJSendAutoData("Message")]
        [InlineJSendAutoData("Code")]
        [InlineJSendAutoData("Data")]
        public void PropertiesAreCorrectlyInitialized(
            string propertyName, ConstructorInitializedMemberAssertion assertion)
        {
            // Fixture setup
            var property = typeof (ErrorResponse).GetProperty(propertyName);
            // Exercise system and verify outcome
            assertion.Verify(property);
        }

        [Theory, JSendAutoData]
        public void CodeIsNullByDefault(string message)
        {
            // Exercise system
            var response = new ErrorResponse(message);
            // Verify outcome
            response.Code.Should().NotHaveValue();
        }

        [Theory, JSendAutoData]
        public void DataIsNullByDefault(string message)
        {
            // Exercise system
            var response = new ErrorResponse(message);
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

            var response = new ErrorResponse(message, code, data);
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            Assert.Equal(expectedSerializedResponse, serializedResponse, JToken.EqualityComparer);
        }

        [Theory, JSendAutoData]
        public void NullCodeIsNotSerialized(string message)
        {
            // Fixture setup
            var response = new ErrorResponse(message);
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            serializedResponse.Should().NotContainKey("code");
        }

        [Theory, JSendAutoData]
        public void NullDataIsNotSerialized(string message)
        {
            // Fixture setup
            var response = new ErrorResponse(message);
            // Exercise system
            var serializedResponse = JObject.FromObject(response);
            // Verify outcome
            serializedResponse.Should().NotContainKey("data");
        }
    }
}
