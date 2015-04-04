using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FluentAssertions;
using JSend.WebApi.Tests.FixtureCustomizations;
using JSend.WebApi.Tests.TestTypes;
using Moq;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class ValueActionFilterAttributeTests
    {
        [Theory, JSendAutoData]
        public void IsActionFilterAttribute(ValueActionFilterAttribute filter)
        {
            // Exercise system and verify outcome
            filter.Should().BeAssignableTo<ActionFilterAttribute>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenActionContextIsNull(ValueActionFilterAttribute filter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => filter.OnActionExecuting(null));
        }

        [Theory, JSendAutoData]
        public void WrapsActionDescriptor_WithDelegatingActionDescriptor_WhenActionReturnsValue(
            IFixture fixture, ValueActionFilterAttribute filter)
        {
            // FIxture  setup
            var initialDescriptor = fixture.Freeze<HttpActionDescriptor>();

            Mock.Get(initialDescriptor)
                .SetupGet(des => des.ReturnType)
                .Returns(typeof (Model));

            var context = fixture.Create<HttpActionContext>();
            // Exercise system
            filter.OnActionExecuting(context);
            // Verify outcome
            var descriptor = context.ActionDescriptor;

            descriptor.Should().BeOfType<DelegatingActionDescriptor>();
            descriptor.As<DelegatingActionDescriptor>()
                .InnerActionDescriptor.Should().Be(initialDescriptor);
        }

        [Theory]
        [InlineJSendAutoData(typeof (string), typeof (JSendValueResultConverter<string>))]
        [InlineJSendAutoData(typeof (Model), typeof (JSendValueResultConverter<Model>))]
        [InlineJSendAutoData(typeof (object), typeof (JSendValueResultConverter<object>))]
        public void ComposesActionDescriptor_WithValueResultConverter_WhenActionReturnsValue(
            Type actionType, Type expectedConverterType, IFixture fixture, ValueActionFilterAttribute filter)
        {
            // Fixture setup
            Mock.Get(fixture.Freeze<HttpActionDescriptor>())
                .SetupGet(des => des.ReturnType)
                .Returns(actionType);

            var context = fixture.Create<HttpActionContext>();
            // Exercise system
            filter.OnActionExecuting(context);
            // Verify outcome
            context.ActionDescriptor.ResultConverter.Should().BeOfType(expectedConverterType);
        }

        [Theory]
        [InlineJSendAutoData(null)]
        [InlineJSendAutoData(typeof (IHttpActionResult))]
        [InlineJSendAutoData(typeof (HttpResponseMessage))]
        public void DoesNotChangeDescriptor_WhenActionDoesNotReturnValue(
            Type actionType, IFixture fixture, ValueActionFilterAttribute filter)
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
