using System;
using System.Web.Http.ExceptionHandling;
using FluentAssertions;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class JSendExceptionHandlerTests
    {
        [Theory, JSendAutoData]
        public void IsExceptionHandler(JSendExceptionHandler handler)
        {
            // Exercise system
            handler.Should().BeAssignableTo<ExceptionHandler>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenContextIsNull(JSendExceptionHandler handler)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => handler.Handle(null));
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenRequestContextIsNull(ExceptionContext exceptionContext, JSendExceptionHandler handler)
        {
            // Fixture setup
            exceptionContext.RequestContext = null;
            var handlerContext = new ExceptionHandlerContext(exceptionContext);
            // Exercise system and verify outcome
            Action handle = () => handler.Handle(handlerContext);
            handle.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain("ExceptionHandlerContext.RequestContext must not be null.");
        }

        [Theory, JSendAutoData]
        public void SetsResultToJSendExceptionResult(ExceptionHandlerContext context, JSendExceptionHandler handler)
        {
            // Exercise system
            handler.Handle(context);
            // Verify outcome
            context.Result.Should().BeAssignableTo<JSendExceptionResult>();
        }
    }
}
