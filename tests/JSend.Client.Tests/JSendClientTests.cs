using System;
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
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

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

            public HttpRequestMessage Request;
            public string Content;

            public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
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

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenClientIsNull(JSendClientSettings settings)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendClient(settings, null));
        }

        [Theory, JSendAutoData]
        public void ClientSettingsCanBeNull()
        {
            // Exercise system and verify outcome
            Assert.DoesNotThrow(() => new JSendClient(null));
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
            client.Parser.Should().BeOfType<DefaultJSendParser>();
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
        public async Task GetAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [FrozenAsHttpClient] HttpClientStub clientStub,
            Uri uri, [Greedy] JSendClient client)
        {
            // Fixture setup
            clientStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<Model>(httpResponseMessage))
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

        [Theory, JSendAutoData]
        public async Task GetAsync_SetsUri(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.GetAsync<Model>(uri);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(uri);
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
                .Setup(p => p.ParseAsync<Model>(httpResponseMessage))
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

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SetsUri(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(uri);
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
                .Setup(p => p.ParseAsync<JToken>(httpResponseMessage))
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

        [Theory, JSendAutoData]
        public async Task PostAsync_SetsUri(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(uri);
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
                .Setup(p => p.ParseAsync<JToken>(httpResponseMessage))
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

        [Theory, JSendAutoData]
        public async Task DeleteAsync_SetsUri(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.DeleteAsync(uri);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(uri);
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
                .Setup(p => p.ParseAsync<Model>(httpResponseMessage))
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

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SetsUri(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(uri);
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
                .Setup(p => p.ParseAsync<JToken>(httpResponseMessage))
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

        [Theory, JSendAutoData]
        public async Task PutAsync_SetsUri(
            [FrozenAsHttpClient] HttpClientSpy httpClientSpy,
            Uri uri, object content, [Greedy] JSendClient client)
        {
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = httpClientSpy.Request;
            request.RequestUri.Should().Be(uri);
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
                .Setup(p => p.ParseAsync<Model>(httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.SendAsync<Model>(request);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
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
