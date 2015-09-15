using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.Parsers;
using JSend.Client.Properties;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.Client.Tests.Parsers
{
    public class NakedContentParserTests
    {

        [Theory, JSendAutoData]
        public void ThrowsWhenHttpResponseMessageIsNull(NakedContentParser parser)
        {
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, null))
                .ShouldThrow<ArgumentNullException>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponseHasNoContent(HttpResponseMessage message, NakedContentParser parser)
        {
            // Fixture setup
            message.Content = null;
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithMessage(StringResources.ResponseWithoutContent);
        }

        [Theory, JSendAutoData]
        public void WrapsJsonExeptionsWithCorrectMessage(HttpResponseMessage message, NakedContentParser parser)
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
        public void ThrowsWhenResponse_IsNotValidJson(HttpResponseMessage message, NakedContentParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("1,2,3");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonReaderException>();
        }

        [Theory, JSendAutoData]
        public async Task ParsesSuccessResponse_WithoutData(HttpResponseMessage message, NakedContentParser parser)
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
            NakedContentParser parser)
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
            NakedContentParser parser)
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

        [Theory, JSendAutoData]
        public async Task ParsesNakedData_AsASuccessResponse(Model model, HttpResponseMessage message,
            NakedContentParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(JsonConvert.SerializeObject(model));
            // Exercise system
            var response = await parser.ParseAsync<Model>(null, message);
            // Verify outcome
            response.Status.Should().Be(JSendStatus.Success);
            response.Data.ShouldBeEquivalentTo(model);
        }

        public class ModelWithPrivateDefaultConstructor
        {
            [JsonIgnore]
            public bool PrivateDefaultConstructorCalled = false;

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
            [NoAutoProperties] DefaultJSendParserTests.ModelWithPrivateDefaultConstructor model,
            HttpResponseMessage message,
            NakedContentParser parser)
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
            var response = await parser.ParseAsync<DefaultJSendParserTests.ModelWithPrivateDefaultConstructor>(serializerSettings, message);
            // Verify outcome
            response.Data.PrivateDefaultConstructorCalled.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenDataIsOfAWrongType(HttpResponseMessage message, NakedContentParser parser)
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
        public void ThrowsWhenNakedDataIsOfAWrongType(HttpResponseMessage message, NakedContentParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("\"invalid data\"");
            // Exercise system and verify outcome
            parser.Awaiting(p => p.ParseAsync<Model>(null, message))
                .ShouldThrow<JSendParseException>()
                .WithInnerException<JsonSerializationException>()
                .WithInnerMessage("Error converting value \"invalid data\"*");
        }

        [Theory, JSendAutoData]
        public async Task ParsesFailResponse(string reason, HttpResponseMessage message, NakedContentParser parser)
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
            HttpResponseMessage message, NakedContentParser parser)
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
        public async Task ParsesErrorResponse_WithAllFields(string message, int code, DateTime data,
            HttpResponseMessage responseMessage, NakedContentParser parser)
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
            HttpResponseMessage responseMessage, NakedContentParser parser)
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
            HttpResponseMessage responseMessage, NakedContentParser parser)
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
            HttpResponseMessage responseMessage, NakedContentParser parser)
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
            HttpResponseMessage responseMessage, NakedContentParser parser)
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
            HttpResponseMessage message, NakedContentParser parser)
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
    }
}
