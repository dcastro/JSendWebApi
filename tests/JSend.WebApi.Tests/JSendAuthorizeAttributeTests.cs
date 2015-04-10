using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Xunit;

namespace JSend.WebApi.Tests
{
    public class JSendAuthorizeAttributeTests
    {
        public class TestableJSendAuthorizeAttribute : JSendAuthorizeAttribute
        {
            public override void OnAuthorization(HttpActionContext actionContext)
            {
                //skip null-check for actionContext at this stage
                //in order to verify that HandleUnauthorizedRequest performs a null-check itself

                //automatically fail authorization
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
        public void CreatesResponse(HttpActionContext context, TestableJSendAuthorizeAttribute attribute)
        {
            // Fixture setup
            context.Response = null;
            // Exercise system
            attribute.OnAuthorization(context);
            // Verify outcome
            context.Response.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public async Task CreatesFailResponse(HttpActionContext context, TestableJSendAuthorizeAttribute attribute)
        {
            // Fixture setup
            var expectedMessage = JsonConvert.SerializeObject(new FailResponse(StringResources.RequestNotAuthorized));
            // Exercise system
            attribute.OnAuthorization(context);
            // Verify outcome
            var message = await context.Response.Content.ReadAsStringAsync();
            message.Should().Be(expectedMessage);
        }

        [Theory, JSendAutoData]
        public void SetsStatusCode(HttpActionContext context, TestableJSendAuthorizeAttribute attribute)
        {
            // Exercise system
            attribute.OnAuthorization(context);
            // Verify outcome
            context.Response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory, JSendAutoData]
        public void SetsContentTypeHeader(HttpActionContext context, TestableJSendAuthorizeAttribute attribute)
        {
            // Exercise system
            attribute.OnAuthorization(context);
            // Verify outcome
            context.Response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
