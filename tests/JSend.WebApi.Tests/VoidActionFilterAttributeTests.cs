using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FluentAssertions;
using JSend.WebApi.Tests.FixtureCustomizations;
using Moq;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class VoidActionFilterAttributeTests
    {
        [Theory, JSendAutoData]
        public void IsActionFilter(VoidActionFilterAttribute filter)
        {
            // Exercise system and verify outcome
            filter.Should().BeAssignableTo<ActionFilterAttribute>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenActionContextIsNull(VoidActionFilterAttribute filter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => filter.OnActionExecuting(null));
        }

        [Theory, JSendAutoData]
        public void WrapsActionDescriptor_WithDelegatingActionDescriptor_WhenActionReturnsVoid(
            IFixture fixture, VoidActionFilterAttribute filter)
        {
            // FIxture  setup
            var initialDescriptor = fixture.Freeze<HttpActionDescriptor>();

            Mock.Get(initialDescriptor)
                .SetupGet(des => des.ReturnType)
                .Returns(null as Type);

            var context = fixture.Create<HttpActionContext>();
            // Exercise system
            filter.OnActionExecuting(context);
            // Verify outcome
            var descriptor = context.ActionDescriptor;

            descriptor.Should().BeOfType<DelegatingActionDescriptor>();
            descriptor.As<DelegatingActionDescriptor>()
                .InnerActionDescriptor.Should().Be(initialDescriptor);
        }

        [Theory, JSendAutoData]
        public void ComposesActionDescriptor_WithVoidResultConverter_WhenActionReturnsVoid(
            IFixture fixture, VoidActionFilterAttribute filter)
        {
            // Fixture setup
            Mock.Get(fixture.Freeze<HttpActionDescriptor>())
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
            Type actionType, IFixture fixture, VoidActionFilterAttribute filter)
        {
            // Fixture setup
            Mock.Get(fixture.Freeze<HttpActionDescriptor>())
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
