using System.Collections.Generic;
using System.Threading;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSend.WebApi.Tests.FixtureCustomizations;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
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
        public void InitializesConverterCorrectly(HttpActionDescriptor innerDescriptor,
            IActionResultConverter converter)
        {
            // Exercise system
            var descriptor = new DelegatingActionDescriptor(innerDescriptor, converter);
            // Verify outcome
            descriptor.ResultConverter.Should().BeSameAs(converter);
        }

        [Theory, JSendAutoData]
        public void InitializesInnerActionDescriptorCorrectly(HttpActionDescriptor innerDescriptor,
            IActionResultConverter converter)
        {
            // Exercise system
            var descriptor = new DelegatingActionDescriptor(innerDescriptor, converter);
            // Verify outcome
            descriptor.InnerActionDescriptor.Should().Be(innerDescriptor);
        }

        [Theory, JSendAutoData]
        public void DelegatesGetParameters(DelegatingActionDescriptor descriptor)
        {
            // Fixture setup
            var expectedParameters = descriptor.InnerActionDescriptor.GetParameters();
            // Exercise system
            var parameters = descriptor.GetParameters();
            // Verify outcome
            parameters.Should().BeSameAs(expectedParameters);
        }

        [Theory, JSendAutoData]
        public void DelegatesExecuteAsync(
            [NoAutoProperties] HttpControllerContext context, IDictionary<string, object> args, CancellationToken token,
            DelegatingActionDescriptor descriptor)
        {
            // Fixture setup
            var expectedTask = descriptor.InnerActionDescriptor.ExecuteAsync(context, args, token);
            // Exercise system
            var task = descriptor.ExecuteAsync(context, args, token);
            // Verify outcome
            task.Should().BeSameAs(expectedTask);
        }

        [Theory, JSendAutoData]
        public void DelegatesActionName(DelegatingActionDescriptor descriptor)
        {
            // Fixture setup
            var expectedActionName = descriptor.InnerActionDescriptor.ActionName;
            // Exercise system
            var actionName = descriptor.ActionName;
            // Verify outcome
            actionName.Should().BeSameAs(expectedActionName);
        }

        [Theory, JSendAutoData]
        public void DelegatesReturnType(DelegatingActionDescriptor descriptor)
        {
            // Fixture setup
            var expectedReturnType = descriptor.InnerActionDescriptor.ReturnType;
            // Exercise system
            var returnType = descriptor.ReturnType;
            // Verify outcome
            returnType.Should().BeSameAs(expectedReturnType);
        }
    }
}
