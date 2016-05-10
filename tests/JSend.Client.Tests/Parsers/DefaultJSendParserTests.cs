using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.Parsers;
using JSend.Client.Properties;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.Client.Tests.Parsers
{
    public class DefaultJSendParserTests
    {
        [Fact]
        public void InstanceIsSingleton()
        {
            // Exercise system
            var first = DefaultJSendParser.Instance;
            var second = DefaultJSendParser.Instance;
            // Verify outcome
            first.Should().BeSameAs(second);
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenHttpResponseMessageIsNull(DefaultJSendParser parser)
        {
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, null))
                .ShouldThrow<ArgumentNullException>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponseHasNoContent(DefaultJSendParser parser)
        {
            // Fixture setup
            var message = new HttpResponseMessage();
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithMessage(StringResources.ResponseWithEmptyBody);
        }

        [Theory]
        [InlineJSendAutoDataAttribute("")]
        [InlineJSendAutoDataAttribute("  ")]
        [InlineJSendAutoDataAttribute("\r\n")]
        [InlineJSendAutoDataAttribute("\n")]
        public void ThrowsWhenResponseHasEmptyBody(string content, HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(content);
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithMessage(StringResources.ResponseWithEmptyBody);
        }

        [Theory, JSendAutoData]
        public async Task Parses204NoContent(DefaultJSendParser parser)
        {
            // Fixture setup
            var message = new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                Content = new StringContent("")
            };
            // Exercise system and verify outcome
            var response = await parser.ParseAsync<Model>(null, message);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Success);
            response.HasData.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void WrapsJsonExeptionsWithCorrectMessage(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            const string invalidContent = "qwerty";

            var expectedMessage =
                @"HTTP response message could not be parsed into an instance of type ""JSend.Client.JSendResponse`1[JSend.Client.Tests.TestTypes.Model]"". Content:" +
                Environment.NewLine +
                invalidContent;

            message.Content = new StringContent(invalidContent);
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithMessage(expectedMessage)
                .WithInnerException<JsonException>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponse_IsNotValidJson(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("1,2,3");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonReaderException>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponse_IsJsonArray(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("[1,2,3]");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Invalid type. Expected Object but got Array*");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponse_IsNullToken(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("null");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Invalid type. Expected Object but got Null*");
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Value 123 is not defined in enum.");
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
            var response = await parser.ParseAsync<Model>(null, message);
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
            var response = await parser.ParseAsync<int>(null, message);
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
            var response = await parser.ParseAsync<Model>(null, message);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Success);
            response.Data.ShouldBeEquivalentTo(model);
        }

        public class ModelWithPrivateDefaultConstructor
        {
            [JsonIgnore] public bool PrivateDefaultConstructorCalled = false;

            private ModelWithPrivateDefaultConstructor()
            {
                PrivateDefaultConstructorCalled = true;
            }

            public ModelWithPrivateDefaultConstructor(string s)
            {
            }
        }

        [Theory, JSendAutoData]
        public async Task ParsesSuccessResponse_WithData_UsingSerializerSettings(
            [NoAutoProperties] ModelWithPrivateDefaultConstructor model,
            HttpResponseMessage message,
            DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""success"",
                ""data"": " + JsonConvert.SerializeObject(model) + @"
            }");

            var serializerSettings = new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            // Exercise system
            var response = await parser.ParseAsync<ModelWithPrivateDefaultConstructor>(serializerSettings, message);
            // Verify outcome
            response.Data.PrivateDefaultConstructorCalled.Should().BeTrue();
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            var response = await parser.ParseAsync<Model>(null, message);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Fail);
            response.Error.Data.Type.Should().Be(JTokenType.String);
            response.Error.Data.Value<string>().Should().Be(reason);
        }

        [Theory, JSendAutoData]
        public async Task ParsesFailResponse_UsingSerializerSettings(
            HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""fail"",
                ""data"": ""1989-01-02T15:14:03.9030824""
            }");

            var serializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            // Exercise system
            var response = await parser.ParseAsync<Model>(serializerSettings, message);
            // Verify outcome
            var date = response.Error.Data.Value<DateTime>();
            date.Kind.Should().Be(DateTimeKind.Utc);
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            var response = await parser.ParseAsync<Model>(null, responseMessage);
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
            var response = await parser.ParseAsync<Model>(null, responseMessage);
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
            var response = await parser.ParseAsync<Model>(null, responseMessage);
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
            var response = await parser.ParseAsync<Model>(null, responseMessage);
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
            var response = await parser.ParseAsync<Model>(null, responseMessage);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Error);
            response.Error.ShouldBeEquivalentTo(expectedError);
        }

        [Theory, JSendAutoData]
        public async Task ParsesErrorResponse_UsingSerializerSettings(
            HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""error"",
                ""message"": ""message"",
                ""data"": ""1989-01-02T15:14:03.9030824""
            }");

            var serializerSettings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            // Exercise system
            var response = await parser.ParseAsync<Model>(serializerSettings, message);
            // Verify outcome
            var date = response.Error.Data.Value<DateTime>();
            date.Kind.Should().Be(DateTimeKind.Utc);
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
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
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSchemaException>()
                .WithInnerMessage("Invalid type. Expected Integer, Null but got String*");
        }
    }
}
