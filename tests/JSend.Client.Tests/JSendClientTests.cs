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
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class JSendClientTests
    {
        public class HttpMessageHandlerStub : HttpMessageHandler
        {
            public HttpResponseMessage ReturnOnSend;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(ReturnOnSend);
            }
        }

        public class HttpMessageHandlerSpy : HttpMessageHandler
        {
            public HttpRequestMessage Request;
            public string Content;

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Request = request;

                //read the content before the request gets disposed of
                if (request.Content != null)
                    Content = await request.Content.ReadAsStringAsync();

                return new HttpResponseMessage();
            }
        }

        private class WithHandler : CustomizeAttribute, ICustomization
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return this;
            }

            public void Customize(IFixture fixture)
            {
                // Force AutoFixture to inject a message handler into HttpClient
                // and a HttpClient factory into the JSendClient
                fixture.Customize<HttpClient>(c => c.FromFactory(
                    new MethodInvoker(new GreedyConstructorQuery())));
                fixture.Customize<JSendClient>(c => c.FromFactory(
                    new MethodInvoker(new GreedyConstructorQuery())));
            }
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenClientFactoryIsNull(JSendClientSettings settings)
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
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            Uri uri, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

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
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.GetAsync<Model>(uri);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Method.Should().Be(HttpMethod.Get);
        }

        [Theory, JSendAutoData]
        public async Task GetAsync_SetsUri(
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.GetAsync<Model>(uri);
            // Verify outcome
            var request = handlerSpy.Request;
            request.RequestUri.Should().Be(uri);
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            Uri uri, Model content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

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
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Method.Should().Be(HttpMethod.Post);
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SetsUri(
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.RequestUri.Should().Be(uri);
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SerializesContent(
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, Model content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(content);
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var actualContent = handlerSpy.Content;
            actualContent.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SetsContentTypeHeader(
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task GenericPostAsync_SetsCharSet(
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            var expectedCharSet = client.Encoding.WebName;
            // Exercise system
            await client.PostAsync<object>(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Content.Headers.ContentType.CharSet.Should().Be(expectedCharSet);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<object> parsedResponse,
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            Uri uri, Model content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<object>(httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.PostAsync(uri, content);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SendsPostRequest(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Method.Should().Be(HttpMethod.Post);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SetsUri(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.RequestUri.Should().Be(uri);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SerializesContent(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, Model content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(content);
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var actualContent = handlerSpy.Content;
            actualContent.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SetsContentTypeHeader(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task PostAsync_SetsCharSet(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            var expectedCharSet = client.Encoding.WebName;
            // Exercise system
            await client.PostAsync(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Content.Headers.ContentType.CharSet.Should().Be(expectedCharSet);
        }

        [Theory, JSendAutoData]
        public async Task DeleteAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<object> parsedResponse,
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            Uri uri, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<object>(httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.DeleteAsync(uri);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task DeleteAsync_SendsPostRequest(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.DeleteAsync(uri);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Method.Should().Be(HttpMethod.Delete);
        }

        [Theory, JSendAutoData]
        public async Task DeleteAsync_SetsUri(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.DeleteAsync(uri);
            // Verify outcome
            var request = handlerSpy.Request;
            request.RequestUri.Should().Be(uri);
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            Uri uri, Model content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

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
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Method.Should().Be(HttpMethod.Put);
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SetsUri(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.RequestUri.Should().Be(uri);
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SerializesContent(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, Model content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(content);
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var actualContent = handlerSpy.Content;
            actualContent.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SetsContentTypeHeader(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task GenericPutAsync_SetsCharSet(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            var expectedCharSet = client.Encoding.WebName;
            // Exercise system
            await client.PutAsync<object>(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Content.Headers.ContentType.CharSet.Should().Be(expectedCharSet);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<object> parsedResponse,
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            Uri uri, Model content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<object>(httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.PutAsync(uri, content);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SendsPutRequest(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Method.Should().Be(HttpMethod.Put);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SetsUri(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.RequestUri.Should().Be(uri);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SerializesContent(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, Model content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            var expectedContent = JsonConvert.SerializeObject(content);
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var actualContent = handlerSpy.Content;
            actualContent.Should().Be(expectedContent);
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SetsContentTypeHeader(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task PutAsync_SetsCharSet(
            [Frozen(As = typeof(HttpMessageHandler))] HttpMessageHandlerSpy handlerSpy,
            Uri uri, object content, [WithHandler] JSendClient client)
        {
            // Fixture setup
            var expectedCharSet = client.Encoding.WebName;
            // Exercise system
            await client.PutAsync(uri, content);
            // Verify outcome
            var request = handlerSpy.Request;
            request.Content.Headers.ContentType.CharSet.Should().Be(expectedCharSet);
        }

        [Theory, JSendAutoData]
        public async Task SendAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            HttpRequestMessage request, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(client.Parser)
                .Setup(p => p.ParseAsync<Model>(httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.SendAsync<Model>(request);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }
    }
}
