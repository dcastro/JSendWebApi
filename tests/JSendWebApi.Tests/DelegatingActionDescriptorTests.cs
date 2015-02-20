using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSendWebApi.Tests.FixtureCustomizations;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests
{
    public class DelegatingActionDescriptorTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionDescriptor(DelegatingActionDescriptor descriptor)
        {
            // Exercise system and verify outcome
            descriptor.Should().BeAssignableTo<HttpActionDescriptor>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (DelegatingActionDescriptor).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void InitializesConverterCorrectly(HttpActionDescriptor wrappedDescriptor,
            IActionResultConverter converter)
        {
            // Exercise system
            var descriptor = new DelegatingActionDescriptor(wrappedDescriptor, converter);
            // Verify outcome
            descriptor.ResultConverter.Should().BeSameAs(converter);
        }

        [Theory, JSendAutoData]
        public void DelegatesGetParameters([Frozen] HttpActionDescriptor wrappedDescriptor,
            DelegatingActionDescriptor descriptor)
        {
            // Exercise system
            var result = descriptor.GetParameters();
            // Verify outcome
            result.Should().BeSameAs(wrappedDescriptor.GetParameters());
        }

        [Theory, JSendAutoData]
        public void DelegatesExecuteAsync(
            [NoAutoProperties] HttpControllerContext context, IDictionary<string, object> args, CancellationToken token,
            [Frozen] HttpActionDescriptor wrappedDescriptor, DelegatingActionDescriptor descriptor)
        {
            // Exercise system
            var result = descriptor.ExecuteAsync(null, args, token);
            // Verify outcome
            result.Should().BeSameAs(wrappedDescriptor.ExecuteAsync(context, args, token));
        }

        [Theory, JSendAutoData]
        public void DelegatesActionName([Frozen] HttpActionDescriptor wrappedDescriptor,
            DelegatingActionDescriptor descriptor)
        {
            // Exercise system
            var result = descriptor.ActionName;
            // Verify outcome
            result.Should().BeSameAs(wrappedDescriptor.ActionName);
        }

        [Theory, JSendAutoData]
        public void DelegatesReturnType([Frozen] HttpActionDescriptor wrappedDescriptor,
            DelegatingActionDescriptor descriptor)
        {
            // Exercise system
            var result = descriptor.ReturnType;
            // Verify outcome
            result.Should().BeSameAs(wrappedDescriptor.ReturnType);
        }
    }
}
