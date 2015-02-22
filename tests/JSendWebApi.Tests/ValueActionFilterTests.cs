using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FluentAssertions;
using JSendWebApi.Tests.FixtureCustomizations;
using JSendWebApi.Tests.TestTypes;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace JSendWebApi.Tests
{
    public class ValueActionFilterTests
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
        public void IsActionFilterAttribute(ValueActionFilter filter)
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
            assertion.Verify(typeof (ValueActionFilter).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void WrapsActionDescriptor_WithDelegatingActionDescriptor_WhenActionReturnsValue(
            IFixture fixture, ValueActionFilter filter)
        {
            // FIxture  setup
            fixture.Customize(new HttpActionContextCustomization());
            var initialDescriptorMock = fixture.Freeze<Mock<HttpActionDescriptor>>();
            initialDescriptorMock.SetupGet(des => des.ReturnType)
                .Returns(typeof (Model));

            var context = fixture.Create<HttpActionContext>();
            // Exercise system
            filter.OnActionExecuting(context);
            // Verify outcome
            var descriptor = context.ActionDescriptor;

            descriptor.Should().BeOfType<DelegatingActionDescriptor>();
            descriptor.As<DelegatingActionDescriptor>()
                .InnerActionDescriptor.Should().Be(initialDescriptorMock.Object);
        }

        [Theory]
        [InlineJSendAutoData(typeof (string), typeof (JSendValueResultConverter<string>))]
        [InlineJSendAutoData(typeof (Model), typeof (JSendValueResultConverter<Model>))]
        [InlineJSendAutoData(typeof (object), typeof (JSendValueResultConverter<object>))]
        public void ComposesActionDescriptor_WithValueResultConverter_WhenActionReturnsValue(
            Type actionType, Type expectedConverterType, IFixture fixture, ValueActionFilter filter)
        {
            // Fixture setup
            fixture.Customize(new HttpActionContextCustomization());
            fixture.Freeze<Mock<HttpActionDescriptor>>()
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
            Type actionType, IFixture fixture, ValueActionFilter filter)
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
