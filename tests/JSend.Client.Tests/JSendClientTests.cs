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
        public abstract class HttpMessageHandlerStub : HttpMessageHandler
        {
            public abstract HttpResponseMessage Send();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(Send());
            }
        }

        private class WithFakeClientAttribute : CustomizeAttribute, ICustomization
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return this;
            }

            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new TypeRelay(typeof (HttpContent), typeof (StringContent)));

                fixture.Customizations.Add(
                    new TypeRelay(typeof (HttpMessageHandler), typeof (HttpMessageHandlerStub)));

                fixture.Customize<HttpClient>(
                    c => c.FromFactory(
                        new MethodInvoker(new GreedyConstructorQuery())));

                fixture.Customize<JSendClient>(
                    c => c.FromFactory(
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
            [Frozen] HttpMessageHandlerStub handlerStub, [Frozen] IJSendParser parser,
            Uri uri, [WithFakeClient] JSendClient client)
        {
            // Fixture setup
            Mock.Get(handlerStub)
                .Setup(h => h.Send())
                .Returns(httpResponseMessage);

            Mock.Get(parser)
                .Setup(p => p.ParseAsync<Model>(httpResponseMessage))
                .ReturnsAsync(parsedResponse);
            // Exercise system
            var response = await client.GetAsync<Model>(uri);
            // Verify outcome
            response.Should().BeSameAs(parsedResponse);
        }

        [Theory, JSendAutoData]
        public async Task SendAsync_ReturnsParsedResponse(
            HttpResponseMessage httpResponseMessage, JSendResponse<Model> parsedResponse,
            [Frozen] HttpMessageHandlerStub handlerStub, [Frozen] IJSendParser parser,
            HttpRequestMessage request, [WithFakeClient] JSendClient client)
        {
            // Fixture setup
            Mock.Get(handlerStub)
                .Setup(h => h.Send())
                .Returns(httpResponseMessage);

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
