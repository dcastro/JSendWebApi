using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FluentAssertions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Xunit;

namespace JSend.WebApi.Tests
{
    public class JSendExceptionFilterAttributeTests
    {
        [Theory, JSendAutoData]
        public void IsExceptionFilterAttribute(JSendExceptionFilterAttribute filter)
        {
            // Exercise system
            filter.Should().BeAssignableTo<ExceptionFilterAttribute>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenContextIsNull(JSendExceptionFilterAttribute filter)
        {
            // Exercise system and verify outcome
            filter.Awaiting(f => f.OnExceptionAsync(null, CancellationToken.None))
                .ShouldThrow<ArgumentNullException>();
        }

        [Theory, JSendAutoData]
        public async Task CreatesResponse(HttpActionExecutedContext context, JSendExceptionFilterAttribute filter)
        {
            // Fixture setup
            context.Response = null;
            // Exercise system
            await filter.OnExceptionAsync(context, CancellationToken.None);
            // Verify outcome
            context.Response.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public async Task CreatesErrorResponseWithDetails(HttpRequestContext requestContext,
            HttpActionExecutedContext context, JSendExceptionFilterAttribute filter)
        {
            // Fixture setup
            requestContext.IncludeErrorDetail = true;
            context.Request.SetRequestContext(requestContext);

            var expectedMessage =
                JsonConvert.SerializeObject(new ErrorResponse(context.Exception.Message, context.Exception.ToString()));
            // Exercise system
            await filter.OnExceptionAsync(context, CancellationToken.None);
            // Verify outcome
            var message = await context.Response.Content.ReadAsStringAsync();
            message.Should().Be(expectedMessage);
        }

        [Theory, JSendAutoData]
        public async Task CreatesErrorResponseWithoutDetails(HttpRequestContext requestContext,
            HttpActionExecutedContext context, JSendExceptionFilterAttribute filter)
        {
            // Fixture setup
            requestContext.IncludeErrorDetail = false;
            context.Request.SetRequestContext(requestContext);

            var expectedMessage =
                JsonConvert.SerializeObject(new ErrorResponse(StringResources.DefaultErrorMessage));
            // Exercise system
            await filter.OnExceptionAsync(context, CancellationToken.None);
            // Verify outcome
            var message = await context.Response.Content.ReadAsStringAsync();
            message.Should().Be(expectedMessage);
        }

        [Theory, JSendAutoData]
        public async Task SetsStatusCode(HttpActionExecutedContext context, JSendExceptionFilterAttribute filter)
        {
            // Exercise system
            await filter.OnExceptionAsync(context, CancellationToken.None);
            // Verify outcome
            context.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(HttpActionExecutedContext context, JSendExceptionFilterAttribute filter)
        {
            // Exercise system
            await filter.OnExceptionAsync(context, CancellationToken.None);
            // Verify outcome
            context.Response.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
