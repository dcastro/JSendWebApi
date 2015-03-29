using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
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
            public HttpRequestMessage Sent;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Sent = request;

                return Task.FromResult(new HttpResponseMessage());
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
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendClient).GetConstructors());
        }

        [Theory, JSendAutoData]
        public async Task GetAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            [Frozen] IJSendParser parser, Uri uri, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(parser)
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
            var request = handlerSpy.Sent;
            request.Method.Should().Be(HttpMethod.Get);
        }

        [Theory, JSendAutoData]
        public async Task SendAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [Frozen(As = typeof (HttpMessageHandler))] HttpMessageHandlerStub handlerStub,
            [Frozen] IJSendParser parser, HttpRequestMessage request, [WithHandler] JSendClient client)
        {
            // Fixture setup
            handlerStub.ReturnOnSend = httpResponseMessage;

            Mock.Get(parser)
                .Setup(p => p.ParseAsync<Model>(httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.SendAsync<Model>(request);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }
    }
}
