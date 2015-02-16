using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Results
{
    public class JSendCreatedAtRouteResultTests
    {
        private const string RouteLink = "http://localhost/models/";

        private class RouteCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var urlFactoryMock = new Mock<UrlHelper>();
                urlFactoryMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                    .Returns((string name, IDictionary<string, object> values) => RouteLink);

                fixture.Inject(urlFactoryMock.Object);
            }
        }

        [Theory, JSendAutoData]
        public void IsHttpActionResult(IFixture fixture)
        {
            // Fixture setup
            fixture.Customize(new RouteCustomization());
            // Exercise system
            var result = fixture.Create<JSendCreatedAtRouteResult<Model>>();
            // Verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(IFixture fixture)
        {
            // Fixture setup
            fixture.Customize(new RouteCustomization());

            var routeName = fixture.Create<string>();
            var routeValues = fixture.Create<Dictionary<string, object>>();
            var content = fixture.Create<Model>();
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendCreatedAtRouteResult<Model>(null, routeName, routeValues, content));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenContentIsNull(IFixture fixture)
        {
            // Fixture setup
            fixture.Customize(new RouteCustomization());

            var controller = fixture.Create<JSendApiController>();
            var routeName = fixture.Create<string>();
            var routeValues = fixture.Create<Dictionary<string, object>>();
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendCreatedAtRouteResult<Model>(controller, routeName, routeValues, null));
        }

        [Theory, JSendAutoData]
        public async Task ReturnsSuccessJSendResponse(IFixture fixture)
        {
            // Fixture setup
            fixture.Customize(new RouteCustomization());
            var model = fixture.Freeze<Model>();
            var jsendSuccess = JsonConvert.SerializeObject(new SuccessJSendResponse<Model>(model));

            var result = fixture.Create<JSendCreatedAtRouteResult<Model>>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }

        [Theory, JSendAutoData]
        public async Task StatusCodeIs201(IFixture fixture)
        {
            // Fixture setup
            fixture.Customize(new RouteCustomization());

            var result = fixture.Create<JSendCreatedAtRouteResult<Model>>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory, JSendAutoData]
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            fixture.Customize(new RouteCustomization());
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendCreatedAtRouteResult<Model>>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(IFixture fixture)
        {
            // Fixture setup
            fixture.Customize(new RouteCustomization());

            var result = fixture.Create<JSendCreatedAtRouteResult<Model>>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task SetsLocationHeader(IFixture fixture)
        {
            // Fixture setup
            fixture.Customize(new RouteCustomization());

            var result = fixture.Create<JSendCreatedAtRouteResult<Model>>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Headers.Location.Should().Be(RouteLink);
        }
    }
}
