using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Results;
using JSendWebApi.Tests.FixtureCustomizations;
using JSendWebApi.Tests.TestTypes;
using Moq;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Results
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
                () => new JSendRedirectToRouteResult(null, routeName, routeValues));
        }

        [Theory, JSendAutoData]
        public void ResponseIsInitialized(JSendRedirectToRouteResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public void ResponseIsSuccess(JSendRedirectToRouteResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().BeAssignableTo<SuccessResponse>();
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
        public void ResponseDataIsCorrectlySet(JSendRedirectToRouteResult result)
        {
            // Exercise system and verify outcome
            result.Response.Data.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public async Task StatusCodeIs302(JSendRedirectToRouteResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Theory, JSendAutoData]
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendRedirectToRouteResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
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
            message.Headers.Location.Should().Be(UrlHelperCustomization.RouteLink);
        }
    }
}
