using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSend.WebApi.Tests.FixtureCustomizations;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class JSendAuthorizeAttributeResult
    {
        public class TestableJSendAuthorizeAttribute : JSendAuthorizeAttribute
        {
            public override void OnAuthorization(HttpActionContext actionContext)
            {
                //skip null-check for actionContext at this stage
                //in order to verify that HandleUnauthorizedRequest performs a null-check itself
                base.HandleUnauthorizedRequest(actionContext);
            }
        }

        [Theory, JSendAutoData]
        public void IsAuthorizeAttribute(JSendAuthorizeAttribute attribute)
        {
            // Exercise system and verify outcome
            attribute.Should().BeAssignableTo<JSendAuthorizeAttribute>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenContextIsNull(TestableJSendAuthorizeAttribute attribute)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => attribute.OnAuthorization(null));
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenControllerHasNoJsonFormatter(HttpActionContext context, JSendAuthorizeAttribute attribute)
        {
            // Fixture setup
            var formatters = context.ControllerContext.Configuration.Formatters;
            formatters.OfType<JsonMediaTypeFormatter>().ToList()
                .ForEach(f => formatters.Remove(f));

            var expectedMessage = string.Format("The controller's configuration must contain a formatter of type {0}.",
                typeof (JsonMediaTypeFormatter).FullName);
            // Exercise system and verify outcome
            Action onAuthorization = () => attribute.OnAuthorization(context);
            onAuthorization.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain(expectedMessage);
        }
    }
}
