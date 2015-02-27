using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FluentAssertions;
using JSend.WebApi.Tests.FixtureCustomizations;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class VoidActionFilterTests
    {
        private class HttpActionContextCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<HttpActionContext>(
                    c => c.OmitAutoProperties()
                        .With(ctx => ctx.ActionDescriptor));
            }
        }

        [Theory, JSendAutoData]
        public void IsActionFilterAttribute(VoidActionFilter filter)
        {
            // Exercise system and verify outcome
            filter.Should().BeAssignableTo<ActionFilterAttribute>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenAnyArgumentIsNull(IFixture fixture, GuardClauseAssertion assertion)
        {
            // Fixture setup
            fixture.Customize(new HttpActionContextCustomization());
            // Exercise system and verify outcome
            assertion.Verify(typeof (VoidActionFilter).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void WrapsActionDescriptor_WithDelegatingActionDescriptor_WhenActionReturnsVoid(
            IFixture fixture, VoidActionFilter filter)
        {
            // FIxture  setup
            fixture.Customize(new HttpActionContextCustomization());
            var initialDescriptorMock = fixture.Freeze<Mock<HttpActionDescriptor>>();
            initialDescriptorMock.SetupGet(des => des.ReturnType)
                .Returns(null as Type);

            var context = fixture.Create<HttpActionContext>();
            // Exercise system
            filter.OnActionExecuting(context);
            // Verify outcome
            var descriptor = context.ActionDescriptor;

            descriptor.Should().BeOfType<DelegatingActionDescriptor>();
            descriptor.As<DelegatingActionDescriptor>()
                .InnerActionDescriptor.Should().Be(initialDescriptorMock.Object);
        }

        [Theory, JSendAutoData]
        public void ComposesActionDescriptor_WithVoidResultConverter_WhenActionReturnsVoid(
            IFixture fixture, VoidActionFilter filter)
        {
            // Fixture setup
            fixture.Customize(new HttpActionContextCustomization());
            fixture.Freeze<Mock<HttpActionDescriptor>>()
                .SetupGet(des => des.ReturnType)
                .Returns(null as Type);

            var context = fixture.Create<HttpActionContext>();
            // Exercise system
            filter.OnActionExecuting(context);
            // Verify outcome
            context.ActionDescriptor.ResultConverter.Should().BeAssignableTo<JSendVoidResultConverter>();
        }

        [Theory]
        [InlineJSendAutoData(typeof (string))]
        [InlineJSendAutoData(typeof (IHttpActionResult))]
        [InlineJSendAutoData(typeof (HttpResponseMessage))]
        public void DoesNotChangeDescriptor_WhenActionDoesNotReturnVoid(
            Type actionType, IFixture fixture, VoidActionFilter filter)
        {
            // Fixture setup
            fixture.Customize(new HttpActionContextCustomization());
            fixture.Freeze<Mock<HttpActionDescriptor>>()
                .SetupGet(des => des.ReturnType)
                .Returns(actionType);

            var context = fixture.Create<HttpActionContext>();
            var initialDescriptor = context.ActionDescriptor;
            // Exercise system
            filter.OnActionExecuting(context);
            // Verify outcome
            context.ActionDescriptor.Should().BeSameAs(initialDescriptor);
        }
    }
}
