using System.Collections.Generic;
using System.Net.Http;
using FluentAssertions;
using JSend.Client.Tests.FixtureCustomizations;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class CompositeMessageInterceptorTests
    {
        [Theory, JSendAutoData]
        public void ArgumentsCannotBeNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify  outcome
            assertion.Verify(typeof (CompositeMessageInterceptor).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void Interceptors_AreCorrectlyInitialized_UsingAnArray(MessageInterceptor[] interceptors)
        {
            // Exercise system
            var composite = new CompositeMessageInterceptor(interceptors);
            // Verify outcome
            composite.Interceptors.Should().BeEquivalentTo(interceptors);
        }

        [Theory, JSendAutoData]
        public void Interceptors_AreCorrectlyInitialized_UsingAnEnumerable(List<MessageInterceptor> interceptors)
        {
            // Exercise system
            var composite = new CompositeMessageInterceptor(interceptors);
            // Verify outcome
            composite.Interceptors.Should().BeEquivalentTo(interceptors);
        }

        [Theory, JSendAutoData]
        public void InvokesOnSending_OnAllInterceptors(HttpRequestMessage request, CompositeMessageInterceptor composite)
        {
            // Exercise system
            composite.OnSending(request);
            // Verify outcome
            foreach (var interceptor in composite.Interceptors)
                Mock.Get(interceptor)
                    .Verify(i => i.OnSending(request));
        }

        [Theory, JSendAutoData]
        public void InvokesOnReceived_OnAllInterceptors(
            ResponseReceivedContext context, CompositeMessageInterceptor composite)
        {
            // Exercise system
            composite.OnReceived(context);
            // Verify outcome
            foreach (var interceptor in composite.Interceptors)
                Mock.Get(interceptor)
                    .Verify(i => i.OnReceived(context));
        }

        [Theory, JSendAutoData]
        public void InvokesOnParsed_OnAllInterceptors(
            ResponseParsedContext<object> context, CompositeMessageInterceptor composite)
        {
            // Exercise system
            composite.OnParsed(context);
            // Verify outcome
            foreach (var interceptor in composite.Interceptors)
                Mock.Get(interceptor)
                    .Verify(i => i.OnParsed(context));
        }

        [Theory, JSendAutoData]
        public void InvokesOnException_OnAllInterceptors(ExceptionContext context, CompositeMessageInterceptor composite)
        {
            // Exercise system
            composite.OnException(context);
            // Verify outcome
            foreach (var interceptor in composite.Interceptors)
                Mock.Get(interceptor)
                    .Verify(i => i.OnException(context));
        }
    }
}
