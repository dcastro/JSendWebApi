using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.Client.Tests
{
    public class JSendClientTests
    {
        public class HttpClientStub : HttpClient
        {
            public HttpResponseMessage ReturnOnSend;

            public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(ReturnOnSend);
            }
        }

        public class HttpClientSpy : HttpClient
        {
            public bool Disposed = false;

            public bool HasRequestBeenSent = false;
            public HttpRequestMessage Request;
            public string Content;

            public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                HasRequestBeenSent = true;
                Request = request;

                //read the content before the request gets disposed of
                if (request.Content != null)
                    Content = await request.Content.ReadAsStringAsync();

                return new HttpResponseMessage();
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                Disposed = disposing;
            }
        }

        public class FrozenAsHttpClient : CustomizeAttribute
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                // Freezes a HttpClientStub/HttpClientSpy as HttpClient
                // and prevents AutoFixture from setting their public fields
                return new CompositeCustomization(
                    new NoAutoPropertiesCustomization(parameter.ParameterType),
                    new FreezingCustomization(parameter.ParameterType, typeof (HttpClient)));
            }
        }

        private class MockHttpClient : CustomizeAttribute, ICustomization
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return this;
            }

            public void Customize(IFixture fixture)
            {
                var clientMock = fixture.Create<Mock<HttpClient>>();
                fixture.Inject(clientMock.Object);
            }
        }

        public static IEnumerable<object[]> AbsoluteUri
        {
            get
            {
                yield return new object[] {"http://www.contoso.com/users/", new Uri("http://www.contoso.com/users/")};
            }
        }

        public static IEnumerable<object[]> RelativeUri
        {
            get { yield return new object[] {"users", new Uri("users", UriKind.Relative)}; }
        }

        public static IEnumerable<object[]> EmptyUri
        {
            get { yield return new object[] {"", null}; }
        }

        public static IEnumerable<object[]> NullUri
        {
            get { yield return new object[] {null, null}; }
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenClientIsNull(JSendClientSettings settings)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendClient(settings, null));
        }

        [Theory, JSendAutoData]
        public void ClientSettingsCanBeNull()
        {
            // Exercise system
            Action ctor = () => new JSendClient(null);
            // Verify outcome
            ctor.ShouldNotThrow();
        }

        [Theory, JSendAutoData]
        public void JSendParser_IsCorrectlyInitialized(JSendClientSettings settings)
        {
            // Exercise system
            var client = new JSendClient(settings);
            // Verify outcome
            client.Parser.Should().BeSameAs(settings.Parser);
        }

        [Fact]
        public void JSendParser_IsDefaultJSendParser_ByDefault()
        {
            // Exercise system
            var client = new JSendClient();
            // Verify outcome
            client.Parser.Should().BeSameAs(DefaultJSendParser.Instance);
        }

        [Theory, JSendAutoData]
        public void MessageInterceptor_IsCorrectlyInitialized(JSendClientSettings settings)
        {
            // Exercise system
            var client = new JSendClient(settings);
            // Verify outcome
            client.MessageInterceptor.Should().BeSameAs(settings.MessageInterceptor);
        }

        [Fact]
        public void MessageInterceptor_IsNullMessageInterceptor_ByDefault()
        {
            // Exercise system
            var client = new JSendClient();
            // Verify outcome
            client.MessageInterceptor.Should().BeSameAs(NullMessageInterceptor.Instance);
        }

        [Theory, JSendAutoData]
        public void Encoding_IsCorrectlyInitialized(JSendClientSettings settings)
        {
            // Exercise system
            var client = new JSendClient(settings);
            // Verify outcome
            client.Encoding.Should().BeSameAs(settings.Encoding);
        }

        [Fact]
        public void Encoding_IsNull_ByDefault()
        {
            // Exercise system
            var client = new JSendClient();
            // Verify outcome
            client.Encoding.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public void SerializerSettings_IsCorrectlyInitialized(JSendClientSettings settings)
        {
            // Exercise system
            var client = new JSendClient(settings);
            // Verify outcome
            client.SerializerSettings.Should().BeSameAs(settings.SerializerSettings);
        }

        [Fact]
        public void SerializerSettings_IsNull_ByDefault()
        {
            // Exercise system
            var client = new JSendClient();
            // Verify outcome
            client.SerializerSettings.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public void HttpClient_IsCorrectlyInitialized(HttpClient httpClient)
        {
            // Exercise system
            var client = new JSendClient(null, httpClient);
            // Verify outcome
            client.HttpClient.Should().BeSameAs(httpClient);
        }

        [Fact]
        public void HttpClient_IsNotNull_ByDefault()
        {
            // Exercise system
            var client = new JSendClient();
            // Verify outcome
            client.HttpClient.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public async Task GetAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [FrozenAsHttpClient] HttpClientStub clientStub,
            Uri uri, [Greedy] JSendClient client)
        {
            // Fixture setup
            clientStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<Model>(It.IsAny<JsonSerializerSettings>(), httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.GetAsync<Model>(uri);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task GetAsync_SendsGetRequest(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.GetAsync<Model>(uri);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Method.Should().Be(HttpMethod.Get);
        }

        [Theory]
        [PropertyJSendAutoData("AbsoluteUri")]
        [PropertyJSendAutoData("RelativeUri")]
        [PropertyJSendAutoData("EmptyUri")]
        [PropertyJSendAutoData("NullUri")]
        public async Task GetAsync_SetsUri(
            string uri,
            Uri expectedUri,
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            [Greedy] JSendClient client)
        {
            // Exercise system
            await client.GetAsync<Model>(uri);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(expectedUri);
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [FrozenAsHttpClient] HttpClientStub httpClientStub,
            Uri uri, Model content, [Greedy] JSendClient client)
        {
            // Fixture setup
            httpClientStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<Model>(It.IsAny<JsonSerializerSettings>(), httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.PostAsync<Model>(uri, content);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SendsPostRequest(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Method.Should().Be(HttpMethod.Post);
        }

        [Theory]
        [PropertyJSendAutoData("AbsoluteUri")]
        [PropertyJSendAutoData("RelativeUri")]
        [PropertyJSendAutoData("EmptyUri")]
        [PropertyJSendAutoData("NullUri")]
        public async Task GenericPostAsync_SetsUri(
            string uri,
            Uri expectedUri,
            object content,
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(expectedUri);
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SerializesContent(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, Model content, [Greedy] JSendClient client)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(content);
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var actualContent = httpClientSpy.Content;
            actualContent.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SetsContentTypeHeader(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SetsCharSet(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Fixture setup
            var expectedCharSet = client.Encoding.WebName;
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Content.Headers.ContentType.CharSet.Should().Be(expectedCharSet);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<JToken> parsedResponse,
            [FrozenAsHttpClient] HttpClientStub httpClientStub,
            Uri uri, Model content, [Greedy] JSendClient client)
        {
            // Fixture setup
            httpClientStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<JToken>(It.IsAny<JsonSerializerSettings>(), httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.PostAsync(uri, content);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SendsPostRequest(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Method.Should().Be(HttpMethod.Post);
        }

        [Theory]
        [PropertyJSendAutoData("AbsoluteUri")]
        [PropertyJSendAutoData("RelativeUri")]
        [PropertyJSendAutoData("EmptyUri")]
        [PropertyJSendAutoData("NullUri")]
        public async Task PostAsync_SetsUri(
            string uri,
            Uri expectedUri,
            object content,
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(expectedUri);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SerializesContent(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, Model content, [Greedy] JSendClient client)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(content);
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var actualContent = httpClientSpy.Content;
            actualContent.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SetsContentTypeHeader(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SetsCharSet(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Fixture setup
            var expectedCharSet = client.Encoding.WebName;
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Content.Headers.ContentType.CharSet.Should().Be(expectedCharSet);
        }

        [Theory, JSendAutoData]
        public async Task DeleteAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<JToken> parsedResponse,
            [FrozenAsHttpClient] HttpClientStub httpClientStub,
            Uri uri, [Greedy] JSendClient client)
        {
            // Fixture setup
            httpClientStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<JToken>(It.IsAny<JsonSerializerSettings>(), httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.DeleteAsync(uri);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task DeleteAsync_SendsPostRequest(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.DeleteAsync(uri);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Method.Should().Be(HttpMethod.Delete);
        }

        [Theory]
        [PropertyJSendAutoData("AbsoluteUri")]
        [PropertyJSendAutoData("RelativeUri")]
        [PropertyJSendAutoData("EmptyUri")]
        [PropertyJSendAutoData("NullUri")]
        public async Task DeleteAsync_SetsUri(
            string uri,
            Uri expectedUri,
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            [Greedy] JSendClient client)
        {
            // Exercise system
            await client.DeleteAsync(uri);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(expectedUri);
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [FrozenAsHttpClient] HttpClientStub httpClientStub,
            Uri uri, Model content, [Greedy] JSendClient client)
        {
            // Fixture setup
            httpClientStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<Model>(It.IsAny<JsonSerializerSettings>(), httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.PutAsync<Model>(uri, content);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SendsPutRequest(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Method.Should().Be(HttpMethod.Put);
        }

        [Theory]
        [PropertyJSendAutoData("AbsoluteUri")]
        [PropertyJSendAutoData("RelativeUri")]
        [PropertyJSendAutoData("EmptyUri")]
        [PropertyJSendAutoData("NullUri")]
        public async Task GenericPutAsync_SetsUri(
            string uri,
            Uri expectedUri,
            object content,
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(expectedUri);
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SerializesContent(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, Model content, [Greedy] JSendClient client)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(content);
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var actualContent = httpClientSpy.Content;
            actualContent.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SetsContentTypeHeader(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SetsCharSet(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Fixture setup
            var expectedCharSet = client.Encoding.WebName;
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Content.Headers.ContentType.CharSet.Should().Be(expectedCharSet);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<JToken> parsedResponse,
            [FrozenAsHttpClient] HttpClientStub httpClientStub,
            Uri uri, Model content, [Greedy] JSendClient client)
        {
            // Fixture setup
            httpClientStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<JToken>(It.IsAny<JsonSerializerSettings>(), httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.PutAsync(uri, content);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SendsPutRequest(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Method.Should().Be(HttpMethod.Put);
        }

        [Theory]
        [PropertyJSendAutoData("AbsoluteUri")]
        [PropertyJSendAutoData("RelativeUri")]
        [PropertyJSendAutoData("EmptyUri")]
        [PropertyJSendAutoData("NullUri")]
        public async Task PutAsync_SetsUri(
            string uri,
            Uri expectedUri,
            object content,
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(expectedUri);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SerializesContent(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, Model content, [Greedy] JSendClient client)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(content);
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var actualContent = httpClientSpy.Content;
            actualContent.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SetsContentTypeHeader(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SetsCharSet(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Fixture setup
            var expectedCharSet = client.Encoding.WebName;
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.Content.Headers.ContentType.CharSet.Should().Be(expectedCharSet);
        }

        [Theory, JSendAutoData]
        public async Task SendAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [FrozenAsHttpClient] HttpClientStub httpClientStub,
            HttpRequestMessage request, [Greedy] JSendClient client)
        {
            // Fixture setup
            httpClientStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<Model>(It.IsAny<JsonSerializerSettings>(), httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.SendAsync<Model>(request);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task Executes_OnSending_BeforeSendingRequest(
            HttpRequestMessage request,
            [Frozen] MessageInterceptor interceptor,
            [FrozenAsHttpClient] HttpClientSpy httpClient,
            [Greedy] JSendClient client)
        {
            // Fixture setup
            Mock.Get(interceptor)
                .Setup(i => i.OnSending(It.IsAny<HttpRequestMessage>()))
                .Callback(
                    () => httpClient.HasRequestBeenSent.Should().BeFalse());
            // Exercise system
            await client.SendAsync<object>(request);
            // Verify outcome
            Mock.Get(interceptor)
                .Verify(i => i.OnSending(It.IsAny<HttpRequestMessage>()), Times.Once);
        }

        [Theory, JSendAutoData]
        public async Task Executes_OnSending_WithCorrectRequest(
            HttpRequestMessage request,
            [Frozen] MessageInterceptor interceptor,
            [Greedy, MockHttpClient] JSendClient client)
        {
            // Exercise system
            await client.SendAsync<object>(request);
            // Verify outcome
            Mock.Get(interceptor)
                .Verify(i => i.OnSending(request));
        }

        [Theory, JSendAutoData]
        public async Task Executes_OnReceived_BeforeParsingResponse(
            HttpRequestMessage request,
            [Frozen] MessageInterceptor interceptor,
            [Frozen] IJSendParser parser,
            [Greedy, MockHttpClient] JSendClient client)
        {
            // Fixture setup
            Action verifyResponseHasntBeenParsed = () =>
                Mock.Get(parser)
                    .Verify(
                        p => p.ParseAsync<object>(It.IsAny<JsonSerializerSettings>(), It.IsAny<HttpResponseMessage>()),
                        Times.Never);

            Mock.Get(interceptor)
                .Setup(i => i.OnReceived(It.IsAny<ResponseReceivedContext>()))
                .Callback(verifyResponseHasntBeenParsed);

            // Exercise system
            await client.SendAsync<object>(request);
            // Verify outcome
            Mock.Get(interceptor)
                .Verify(i => i.OnReceived(It.IsAny<ResponseReceivedContext>()), Times.Once);
        }

        [Theory, JSendAutoData]
        public async Task Executes_OnReceived_WithCorrectContext(
            HttpRequestMessage request,
            [Frozen] HttpResponseMessage response,
            [Frozen] MessageInterceptor interceptor,
            [Greedy, MockHttpClient] JSendClient client)
        {
            // Exercise system
            await client.SendAsync<object>(request);
            // Verify outcome
            Mock.Get(interceptor)
                .Verify(i => i.OnReceived(
                    It.Is<ResponseReceivedContext>(
                        ctx => ctx.HttpRequestMessage == request && ctx.HttpResponseMessage == response)));
        }

        [Theory, JSendAutoData]
        public async Task Executes_OnParsed_WithCorrectContext(
            HttpRequestMessage request,
            [Frozen] HttpResponseMessage response,
            [Frozen] JSendResponse<object> jsendResponse,
            [Frozen] MessageInterceptor interceptor,
            [Greedy, MockHttpClient] JSendClient client)
        {
            // Exercise system
            await client.SendAsync<object>(request);
            // Verify outcome
            Mock.Get(interceptor)
                .Verify(i => i.OnParsed<object>(
                    It.Is<ResponseParsedContext<object>>(
                        ctx => ctx.HttpRequestMessage == request &&
                               ctx.HttpResponseMessage == response &&
                               ctx.JSendResponse == jsendResponse)));
        }

        [Theory, JSendAutoData]
        public async Task Executes_OnException_WhenSendingFails_WithCorrectContext(
            Exception exception,
            HttpRequestMessage request,
            [MockHttpClient] HttpClient httpClient,
            [Frozen] MessageInterceptor interceptor,
            [Greedy] JSendClient client)
        {
            // Fixture setup
            Mock.Get(httpClient)
                .Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            // Exercise system
            try
            {
                await client.SendAsync<object>(request);
            }
            catch
            {
            }
            // Verify outcome
            Mock.Get(interceptor)
                .Verify(i => i.OnException(It.Is<ExceptionContext>(
                    ctx => ctx.HttpRequestMessage == request &&
                           ctx.Exception == exception
                    )), Times.Once);
        }

        [Theory, JSendAutoData]
        public async Task Executes_OnException_WhenParsingFails_WithCorrectContext(
            Exception exception,
            HttpRequestMessage request,
            [Frozen] IJSendParser parser,
            [Frozen] MessageInterceptor interceptor,
            [Greedy, MockHttpClient] JSendClient client)
        {
            // Fixture setup
            Mock.Get(parser)
                .Setup(c => c.ParseAsync<object>(It.IsAny<JsonSerializerSettings>(), It.IsAny<HttpResponseMessage>()))
                .ThrowsAsync(exception);
            // Exercise system
            try
            {
                await client.SendAsync<object>(request);
            }
            catch
            {
            }
            // Verify outcome
            Mock.Get(interceptor)
                .Verify(i => i.OnException(It.Is<ExceptionContext>(
                    ctx => ctx.HttpRequestMessage == request &&
                           ctx.Exception == exception
                    )), Times.Once);
        }

        [Theory, JSendAutoData]
        public void ParserExceptionsBubbleUp(
            HttpRequestMessage request,
            JSendParseException exception,
            [Frozen] IJSendParser parser,
            [Greedy, MockHttpClient] JSendClient client)
        {
            // Fixture setup
            Mock.Get(parser)
                .Setup(p => p.ParseAsync<object>(It.IsAny<JsonSerializerSettings>(), It.IsAny<HttpResponseMessage>()))
                .ThrowsAsync(exception);
            // Exercise system and verify outcome
            client
                .Awaiting(c => c.SendAsync<object>(request))
                .ShouldThrow<JSendParseException>();
        }

        [Theory, JSendAutoData]
        public void HttpClientExceptionsAreWrappedAndRethrown(
            HttpRequestMessage request,
            [MockHttpClient] HttpClient httpClient,
            [Greedy] JSendClient client)
        {
            // Fixture setup
            Mock.Get(httpClient)
                .Setup(c => c.SendAsync(request, It.IsAny<CancellationToken>()))
                .Throws<HttpRequestException>();
            // Exercise system and verify outcome
            client
                .Awaiting(c => c.SendAsync<object>(request))
                .ShouldThrow<JSendRequestException>()
                .WithInnerException<HttpRequestException>();
        }

        [Theory, JSendAutoData]
        public void DisposingOfTheJSendClient_DisposesOfTheHttpClient(HttpClientSpy spy)
        {
            // Fixture setup
            var client = new JSendClient(null, spy);
            // Exercise system
            client.Dispose();
            // Verify outcome
            spy.Disposed.Should().BeTrue();
        }
    }
}
