using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class DefaultJSendParserTests
    {
        [Theory, JSendAutoData]
        public void ThrowsWhenHttpResponseMessageIsNull(DefaultJSendParser parser)
        {
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null))
                .ShouldThrow<ArgumentNullException>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponse_IsNotValidJson(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("1,2,3");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonReaderException>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponse_IsJsonArray(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("[1,2,3]");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Invalid type. Expected Object but got Array*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponseDoesNotHaveStatus(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""data"": null
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Required properties are missing from object: status*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenStatusIsInvalid(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""invalid"",
                ""data"": null
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage(@"Value ""invalid"" is not defined in enum*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenStatusIsNotString(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": 123,
                ""data"": null
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Invalid type. Expected String but got Integer*");
        }

        [Theory, JSendAutoData]
        public async Task ParsesSuccessResponse_WithoutData(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""success"",
                ""data"": null
            }");
            // Exercise system
            var response = await parser.ParseAsync<Model>(message);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Success);
            response.HasData.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public async Task ParsesSuccessResponse_WithoutValueTypeData(HttpResponseMessage message,
            DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""success"",
                ""data"": null
            }");
            // Exercise system
            var response = await parser.ParseAsync<int>(message);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Success);
            response.HasData.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public async Task ParsesSuccessResponse_WithData(Model model, HttpResponseMessage message,
            DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""success"",
                ""data"": " + JsonConvert.SerializeObject(model) + @"
            }");
            // Exercise system
            var response = await parser.ParseAsync<Model>(message);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Success);
            response.Data.ShouldBeEquivalentTo(model);
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenSuccessResponse_DoesNotHaveDataKey(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""success""
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Required properties are missing from object: data*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenDataIsOfAWrongType(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""success"",
                ""data"": ""invalid data""
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSerializationException>()
                .WithInnerMessage("Error converting value \"invalid data\"*");
        }

        [Theory, JSendAutoData]
        public async Task ParsesFailResponse(string reason, HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""fail"",
                ""data"": """ + reason + @"""
            }");
            // Exercise system
            var response = await parser.ParseAsync<Model>(message);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Fail);
            response.Error.Data.Type.Should().Be(JTokenType.String);
            response.Error.Data.Value<string>().Should().Be(reason);
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenFailResponse_DoesNotHaveDataKey(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""fail""
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Required properties are missing from object: data*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenFailResponse_HasNullData(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""fail"",
                ""data"": null
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Type Null is disallowed*");
        }

        [Theory, JSendAutoData]
        public async Task ParsesErrorResponse_WithAllFields(string message, int code, DateTime data,
            HttpResponseMessage responseMessage, DefaultJSendParser parser)
        {
            // Fixture setup
            responseMessage.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": """ + message + @""",
                ""code"": " + code + @",
                ""data"": """ + data + @"""
            }");

            var expectedError = new JSendError(JSendStatus.Error, message, code, JToken.FromObject(data));
            // Exercise system
            var response = await parser.ParseAsync<Model>(responseMessage);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Error);
            response.Error.ShouldBeEquivalentTo(expectedError);
        }

        [Theory, JSendAutoData]
        public async Task ParsesErrorResponse_WithoutCodeKey(string message, DateTime data,
            HttpResponseMessage responseMessage, DefaultJSendParser parser)
        {
            // Fixture setup
            responseMessage.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": """ + message + @""",
                ""data"": """ + data + @"""
            }");

            var expectedError = new JSendError(JSendStatus.Error, message, null, JToken.FromObject(data));
            // Exercise system
            var response = await parser.ParseAsync<Model>(responseMessage);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Error);
            response.Error.ShouldBeEquivalentTo(expectedError);
        }

        [Theory, JSendAutoData]
        public async Task ParsesErrorResponse_WithNullCode(string message, DateTime data,
            HttpResponseMessage responseMessage, DefaultJSendParser parser)
        {
            // Fixture setup
            responseMessage.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": """ + message + @""",
                ""code"": null,
                ""data"": """ + data + @"""
            }");

            var expectedError = new JSendError(JSendStatus.Error, message, null, JToken.FromObject(data));
            // Exercise system
            var response = await parser.ParseAsync<Model>(responseMessage);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Error);
            response.Error.ShouldBeEquivalentTo(expectedError);
        }

        [Theory, JSendAutoData]
        public async Task ParsesErrorResponse_WithoutDataKey(string message, int code,
            HttpResponseMessage responseMessage, DefaultJSendParser parser)
        {
            // Fixture setup
            responseMessage.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": """ + message + @""",
                ""code"": " + code + @"
            }");

            var expectedError = new JSendError(JSendStatus.Error, message, code, null);
            // Exercise system
            var response = await parser.ParseAsync<Model>(responseMessage);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Error);
            response.Error.ShouldBeEquivalentTo(expectedError);
        }

        [Theory, JSendAutoData]
        public async Task ParsesErrorResponse_WithNullData(string message, int code,
            HttpResponseMessage responseMessage, DefaultJSendParser parser)
        {
            // Fixture setup
            responseMessage.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": """ + message + @""",
                ""code"": " + code + @",
                ""data"": null
            }");

            var expectedError = new JSendError(JSendStatus.Error, message, code, null);
            // Exercise system
            var response = await parser.ParseAsync<Model>(responseMessage);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Error);
            response.Error.ShouldBeEquivalentTo(expectedError);
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenErrorResponse_DoesNotHaveMessageKey(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""error""
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Required properties are missing from object: message*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenErrorResponse_HasNullMessage(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": null
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Invalid type. Expected String but got Null*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenErrorResponse_HasMessageOfAWrongType(HttpResponseMessage message,
            DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": { }
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Invalid type. Expected String but got Object*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenErrorResponse_HasErrorOfAWrongType(HttpResponseMessage message,
            DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": ""msg"",
                ""code"": ""invalid""
            }");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Invalid type. Expected Integer, Null but got String*");
        }
    }
}
