using System;
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
    }
}
