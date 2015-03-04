using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FluentAssertions;
using JSend.WebApi.Tests.FixtureCustomizations;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class VoidActionFilterTests
    {
        [Theory, JSendAutoData]
        public void IsActionFilter(VoidActionFilter filter)
        {
            // Exercise system and verify outcome
            filter.Should().BeAssignableTo<IActionFilter>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (VoidActionFilter).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenActionContextIsNull(Func<Task<HttpResponseMessage>> continuation, VoidActionFilter filter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => filter.ExecuteActionFilterAsync(null, CancellationToken.None, continuation));
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenContinuationIsNull(HttpActionContext context, VoidActionFilter filter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => filter.ExecuteActionFilterAsync(context, CancellationToken.None, null));
        }

        [Theory, JSendAutoData]
        public async Task WrapsActionDescriptor_WithDelegatingActionDescriptor_WhenActionReturnsVoid(
            IFixture fixture, Func<Task<HttpResponseMessage>> continuation, VoidActionFilter filter)
        {
            // FIxture  setup
            var initialDescriptorMock = fixture.Freeze<Mock<HttpActionDescriptor>>();
            initialDescriptorMock.SetupGet(des => des.ReturnType)
                .Returns(null as Type);

            var context = fixture.Create<HttpActionContext>();
            // Exercise system
            await filter.ExecuteActionFilterAsync(context, CancellationToken.None, continuation);
            // Verify outcome
            var descriptor = context.ActionDescriptor;

            descriptor.Should().BeOfType<DelegatingActionDescriptor>();
            descriptor.As<DelegatingActionDescriptor>()
                .InnerActionDescriptor.Should().Be(initialDescriptorMock.Object);
        }

        [Theory, JSendAutoData]
        public async Task ComposesActionDescriptor_WithVoidResultConverter_WhenActionReturnsVoid(
            IFixture fixture, Func<Task<HttpResponseMessage>> continuation, VoidActionFilter filter)
        {
            // Fixture setup
            fixture.Freeze<Mock<HttpActionDescriptor>>()
                .SetupGet(des => des.ReturnType)
                .Returns(null as Type);

            var context = fixture.Create<HttpActionContext>();
            // Exercise system
            await filter.ExecuteActionFilterAsync(context, CancellationToken.None, continuation);
            // Verify outcome
            context.ActionDescriptor.ResultConverter.Should().BeAssignableTo<JSendVoidResultConverter>();
        }

        [Theory]
        [InlineJSendAutoData(typeof (string))]
        [InlineJSendAutoData(typeof (IHttpActionResult))]
        [InlineJSendAutoData(typeof (HttpResponseMessage))]
        public async Task DoesNotChangeDescriptor_WhenActionDoesNotReturnVoid(
            Type actionType, IFixture fixture, Func<Task<HttpResponseMessage>> continuation, VoidActionFilter filter)
        {
            // Fixture setup
            fixture.Freeze<Mock<HttpActionDescriptor>>()
                .SetupGet(des => des.ReturnType)
                .Returns(actionType);

            var context = fixture.Create<HttpActionContext>();
            var initialDescriptor = context.ActionDescriptor;
            // Exercise system
            await filter.ExecuteActionFilterAsync(context, CancellationToken.None, continuation);
            // Verify outcome
            context.ActionDescriptor.Should().BeSameAs(initialDescriptor);
        }

        [Theory, JSendAutoData]
        public async Task ReturnsContinuationMessage(HttpActionContext context,
            Func<Task<HttpResponseMessage>> continuation, VoidActionFilter filter)
        {
            // Fixture setup
            var continuationMessage = await continuation();
            // Exercise system
            var actualMessage = await filter.ExecuteActionFilterAsync(context, CancellationToken.None, continuation);
            // Verify outcome
            actualMessage.Should().Be(continuationMessage);
        }
    }
}
