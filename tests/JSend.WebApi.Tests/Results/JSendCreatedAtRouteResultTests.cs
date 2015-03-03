using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using JSend.WebApi.Tests.TestTypes;
using Moq;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

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
        public void ResponseIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            Model content, JSendApiController controller)
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
        public void LocationIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            Model content, JSendApiController controller)
        {
            // Fixture setup
            var expectedLocation = new Uri(UrlHelperCustomization.RouteLink);
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.Location.Should().Be(expectedLocation);
        }

        [Theory, JSendAutoData]
        public void ContentIsCorrectlyInitialized(string routeName, Dictionary<string, object> routeValues,
            Model content, JSendApiController controller)
        {
            // Exercise system
            var result = new JSendCreatedAtRouteResult<Model>(routeName, routeValues, content, controller);
            // Verify outcome
            result.Content.Should().Be(content);
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
            message.Headers.Location.Should().Be(UrlHelperCustomization.RouteLink);
        }
    }
}
