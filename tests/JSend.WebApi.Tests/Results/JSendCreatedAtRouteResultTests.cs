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
using JSend.WebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.WebApi.Tests.Results
{
    public class JSendCreatedAtRouteResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendCreatedAtRouteResult<Model> result)
        {
            // Verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(string routeName, Dictionary<string, object> routeValues,
            Model content)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenRouteNameIsNull(
            Dictionary<string, object> routeValues, Model content, ApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendCreatedAtRouteResult<Model>(null, routeValues, content, controller);
            ctor.ShouldThrow<ArgumentNullException>();
        }

        [Theory, JSendAutoData]
        public void CanBeCreatedWithControllerWithoutProperties(
            string routeName, Dictionary<string, object> routeValues, Model content,
            [NoAutoProperties] TestableJSendApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            ctor.ShouldNotThrow();
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            Model content, ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new SuccessResponse(content);
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs201(JSendCreatedAtRouteResult<Model> result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitializedUsingController(
            string routeName, Dictionary<string, object> routeValues, Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.Request.Should().Be(controller.Request);
        }

        [Theory, JSendAutoData]
        public void ContentIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.Content.Should().Be(content);
        }

        [Theory, JSendAutoData]
        public void RouteNameIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.RouteName.Should().Be(routeName);
        }

        [Theory, JSendAutoData]
        public void RouteValuesIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.RouteValues.Should().BeSameAs(routeValues);
        }

        [Theory, JSendAutoData]
        public void UrlFactoryIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            Model content, ApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.UrlFactory.Should().BeSameAs(controller.Url);
        }

        [Theory, JSendAutoData]
        public void UrlFactoryIsCreated_WhenControllerDoesNotHaveAUrlFactory(string routeName, Model content,
            Dictionary<string, object> routeValues, ApiController controller)
        {
            // Fixture setup
            controller.Url = null;
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.UrlFactory.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendCreatedAtRouteResult<Model> result)
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
        public async Task SetsStatusCode(JSendCreatedAtRouteResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendCreatedAtRouteResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task SetsLocationHeader(JSendCreatedAtRouteResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Headers.Location.Should().Be(TestConventions.RouteLink);
        }
    }
}
