using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.FunctionalTests.FixtureCustomizations;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.Client.FunctionalTests
{
    public class MessageInterceptionTests
    {
        private class VersionHeaderInterceptor : MessageInterceptor
        {
            public override void OnSending(HttpRequestMessage request)
            {
                request.Headers.Add("Version", "1");
            }
        }

        private class ReplaceNoContentInterceptor : MessageInterceptor
        {
            public override void OnReceived(ResponseReceivedContext context)
            {
                var response = context.HttpResponseMessage;
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    var content = new JObject
                    {
                        {"status", "success"},
                        {"data", null}
                    };
                    response.StatusCode = HttpStatusCode.OK;
                    response.Content = new StringContent(content.ToString());
                }
            }
        }

        public class InterceptorSpy : MessageInterceptor
        {
            public object ResponseParsedContext;
            public ExceptionContext ExceptionContext;

            public override void OnParsed<TResponse>(ResponseParsedContext<TResponse> context)
            {
                ResponseParsedContext = context;
            }

            public override void OnException(ExceptionContext context)
            {
                ExceptionContext = context;
            }
        }

        private class WithInterceptorAttribute : CustomizeAttribute, ICustomization
        {
            private readonly Type _interceptorType;

            public WithInterceptorAttribute(Type interceptorType)
            {
                _interceptorType = interceptorType;
            }

            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return this;
            }

            public void Customize(IFixture fixture)
            {
                fixture.Customizations.Add(
                    new TypeRelay(
                        typeof (MessageInterceptor),
                        _interceptorType));

                fixture.Customize<JSendClientSettings>(
                    c => c.OmitAutoProperties()
                        .With(settings => settings.MessageInterceptor));
            }
        }

        public class FrozenWithoutAutoPropertiesAttribute : CustomizeAttribute
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return new CompositeCustomization(
                    new NoAutoPropertiesCustomization(parameter.ParameterType),
                    new FreezingCustomization(parameter.ParameterType));
            }
        }

        [Theory, JSendAutoData]
        public async Task InterceptsRequestsBeforeBeingSent(
            [WithInterceptor(typeof (VersionHeaderInterceptor))] JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response =
                await client.GetAsync<Dictionary<string, List<String>>>("http://localhost/users/echo-headers"))
            {
                // Verify outcome
                response.Data.Should().ContainKey("Version")
                    .WhichValue.Should().ContainSingle("1");
            }
        }

        [Theory, JSendAutoData]
        public void InterceptsResponsesBeforeBeingParsed(
            [WithInterceptor(typeof (ReplaceNoContentInterceptor))] JSendClient client)
        {
            using (client)
            {
                // Exercise system and verify outcome
                client.Awaiting(c => c.GetAsync<string>("http://localhost/users/no-content"))
                    .ShouldNotThrow();
            }
        }

        [Theory, JSendAutoData]
        public async Task InterceptsParsedResponses(
            [FrozenWithoutAutoProperties] InterceptorSpy spy,
            [WithInterceptor(typeof (InterceptorSpy))] JSendClient client)
        {
            using (client)
            using (var response = await client.GetAsync<string>("http://localhost/users/get"))
            {
                // Exercise system and verify outcome
                spy.ResponseParsedContext.Should().NotBeNull();

                spy.ResponseParsedContext.As<ResponseParsedContext<string>>()
                    .JSendResponse.Should().Be(response);
            }
        }

        [Theory, JSendAutoData]
        public async Task InterceptsExceptions(
            [FrozenWithoutAutoProperties] InterceptorSpy spy,
            [WithInterceptor(typeof (InterceptorSpy))] JSendClient client)
        {
            using (client)
            {
                try
                {
                    await client.GetAsync<string>("http://localhost/users/no-content");
                }
                catch
                {
                }

                // Exercise system and verify outcome
                spy.ExceptionContext.Should().NotBeNull();

                spy.ExceptionContext.Exception
                    .Should().BeOfType<JSendParseException>();
            }
        }
    }
}
