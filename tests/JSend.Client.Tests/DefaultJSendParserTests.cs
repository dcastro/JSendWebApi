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
            Func<Task<JSendResponse<Model>>> parse = () => parser.ParseAsync<Model>(null);
            parse.ShouldThrow<ArgumentNullException>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponse_IsNotValidJson(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("1,2,3");
            // Exercise system and verify outcome
            Func<Task<JSendResponse<Model>>> parse = () => parser.ParseAsync<Model>(message);
            parse.ShouldThrow<JsonReaderException>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenResponse_IsJsonArray(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent("[1,2,3]");
            // Exercise system and verify outcome
            Func<Task<JSendResponse<Model>>> parse = () => parser.ParseAsync<Model>(message);
            parse.ShouldThrow<JsonSchemaException>()
                .And.Message.Should().Contain("Array");
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
            Func<Task<JSendResponse<Model>>> parse = () => parser.ParseAsync<Model>(message);
            parse.ShouldThrow<JsonSchemaException>()
                .And.Message.Should().Contain("status");
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
            Func<Task<JSendResponse<Model>>> parse = () => parser.ParseAsync<Model>(message);
            parse.ShouldThrow<JsonSchemaException>()
                .And.Message.Should().Contain("invalid");
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
            Func<Task<JSendResponse<Model>>> parse = () => parser.ParseAsync<Model>(message);
            parse.ShouldThrow<JsonSchemaException>()
                .And.Message.Should().Contain("String");
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
            Func<Task<JSendResponse<Model>>> parse = () => parser.ParseAsync<Model>(message);
            parse.ShouldThrow<JsonSchemaException>()
                .And.Message.Should().Contain("data");
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenDataIsOfAWrongType(HttpResponseMessage message, DefaultJSendParser parser)
        {
            // Fixture setup
            message.Content = new StringContent(@"
            {
                ""status"": ""success"",
                ""data"": ""string""
            }");
            // Exercise system and verify outcome
            Func<Task<JSendResponse<Model>>> parse = () => parser.ParseAsync<Model>(message);
            parse.ShouldThrow<JsonSerializationException>()
                .And.Message.Should().Contain("string")
                .And.Contain("Model");
        }

        [Theory, JSendAutoData]
        public void ParseSuccessMessageAsync_ThrowsWhenJsonIsNull(HttpResponseMessage httpResponseMessage,
            DefaultJSendParser parser)
        {
            // Exercise system and verify outcome
            Func<Task<JSendResponse<Model>>> parseSuccess =
                () => parser.ParseSuccessMessageAsync<Model>(null, httpResponseMessage);
            parseSuccess.ShouldThrow<ArgumentNullException>();
        }

        [Theory, JSendAutoData]
        public void ParseSuccessMessageAsync_ThrowsWhenHttpResponseMessageIsNull(JToken json, DefaultJSendParser parser)
        {
            // Exercise system and verify outcome
            Func<Task<JSendResponse<Model>>> parseSuccess =
                () => parser.ParseSuccessMessageAsync<Model>(json, null);
            parseSuccess.ShouldThrow<ArgumentNullException>();
        }
    }
}
