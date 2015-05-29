using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.WebApi.Tests.Results
{
    public class JSendRedirectToRouteResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendRedirectToRouteResult result)
        {
            // Verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(string routeName, Dictionary<string, object> routeValues)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendRedirectToRouteResult(routeName, routeValues, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenRouteNameIsNull(
            Dictionary<string, object> routeValues, ApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendRedirectToRouteResult(null, routeValues, controller);
            ctor.ShouldThrow<ArgumentNullException>();
        }

        [Theory, JSendAutoData]
        public void CanBeCreatedWithControllerWithoutProperties(
            string routeName, Dictionary<string, object> routeValues,
            [NoAutoProperties] TestableJSendApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendRedirectToRouteResult(routeName, routeValues, controller);
            ctor.ShouldNotThrow();
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new SuccessResponse();
            // Exercise system
            var result = new JSendRedirectToRouteResult(routeName, routeValues, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs302(JSendRedirectToRouteResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitializedUsingController(
            string routeName, Dictionary<string, object> routeValues, ApiController controller)
        {
            // Exercise system
            var result = new JSendRedirectToRouteResult(routeName, routeValues, controller);
            // Verify outcome
            result.Request.Should().Be(controller.Request);
        }

        [Theory, JSendAutoData]
        public void RouteNameIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues, ApiController controller)
        {
            // Exercise system
            var result = new JSendRedirectToRouteResult(routeName, routeValues, controller);
            // Verify outcome
            result.RouteName.Should().Be(routeName);
        }

        [Theory, JSendAutoData]
        public void RouteValuesIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues, ApiController controller)
        {
            // Exercise system
            var result = new JSendRedirectToRouteResult(routeName, routeValues, controller);
            // Verify outcome
            result.RouteValues.Should().BeSameAs(routeValues);
        }

        [Theory, JSendAutoData]
        public void UrlFactoryIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues, ApiController controller)
        {
            // Exercise system
            var result = new JSendRedirectToRouteResult(routeName, routeValues, controller);
            // Verify outcome
            result.UrlFactory.Should().BeSameAs(controller.Url);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendRedirectToRouteResult result)
        {
            // Fixture setup
            var serializedResponse = JsonConvert.SerializeObject(result.Response);
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            content.Should().Be(serializedResponse);
        }

        [Theory, JSendAutoData]
        public async Task SetsStatusCode(JSendRedirectToRouteResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendRedirectToRouteResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task SetsLocationHeader(JSendRedirectToRouteResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Headers.Location.Should().Be(TestConventions.RouteLink);
        }
    }
}
