using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;
using FluentAssertions;
using JSendWebApi.Results;
using JSendWebApi.Tests.FixtureCustomizations;
using JSendWebApi.Tests.TestTypes;
using Moq;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests
{
    public class JSendApiControllerTests
    {
        [Theory, JSendAutoData]
        public void JSendApiControllerIsApiController(JSendApiController controller)
        {
            // Exercise system and verify outcome
            controller.Should().BeAssignableTo<ApiController>();
        }

        [Fact]
        public void ConstructorsThrowWhenAnyArgumentIsNull()
        {
            // Fixture setup
            var fixture = new Fixture {OmitAutoProperties = true};
            var assertion = new GuardClauseAssertion(fixture);
            // Exercise system and verify outcome
            assertion.Verify(typeof (TestableJSendApiController).GetConstructors());
        }

        [Fact]
        public void SettingsAndEncodingAreCorrectlyInitialized()
        {
            // Fixture setup
            var settings = new JsonSerializerSettings();
            var encoding = Encoding.UTF8;
            // Exercise system
            var controller = new TestableJSendApiController(settings, encoding);
            // Verify outcome
            controller.JsonSerializerSettings.Should().BeSameAs(settings);
            controller.Encoding.Should().BeSameAs(encoding);
        }

        [Fact]
        public void SettingsAreCorrectlyInitialized()
        {
            // Fixture setup
            var settings = new JsonSerializerSettings();
            // Exercise system
            var controller = new TestableJSendApiController(settings);
            // Verify outcome
            controller.JsonSerializerSettings.Should().BeSameAs(settings);
        }

        [Fact]
        public void EncodingIsUtf8WhenNoArgumentsAreSpecified()
        {
            // Fixture setup
            var controller = new TestableJSendApiController();
            // Exercise system
            var encoding = controller.Encoding;
            // Verify outcome
            encoding.Should().BeOfType<UTF8Encoding>();
        }

        [Fact]
        public void EncodingIsUtf8WhenOnlySettingsAreSpecified()
        {
            // Fixture setup
            var controller = new TestableJSendApiController(new JsonSerializerSettings());
            // Exercise system
            var encoding = controller.Encoding;
            // Verify outcome
            encoding.Should().BeOfType<UTF8Encoding>();
        }

        [Fact]
        public void JsonSerializerSettingsAreDefaultSettings()
        {
            // Fixture setup
            var controller = new TestableJSendApiController();
            // Exercise system
            var settings = controller.JsonSerializerSettings;
            // Verify outcome
            settings.ShouldBeEquivalentTo(new JsonSerializerSettings());
        }

        [Theory, JSendAutoData]
        public void EncodingCanBeSet(JSendApiController controller)
        {
            // Fixture setup
            var expectedEncoding = Encoding.ASCII;
            // Exercise system
            controller.Encoding = expectedEncoding;
            // Verify outcome
            controller.Encoding.Should().BeSameAs(expectedEncoding);
        }

        [Theory, JSendAutoData]
        public void EncodingCannotBeSetToNull(JSendApiController controller)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => controller.Encoding = null);
        }

        [Theory, JSendAutoData]
        public void JsonSerializerSettingsCanBeSet([NoAutoProperties] JsonSerializerSettings expectedSettings, JSendApiController controller)
        {
            // Exercise system
            controller.JsonSerializerSettings = expectedSettings;
            // Verify outcome
            controller.JsonSerializerSettings.Should().BeSameAs(expectedSettings);
        }

        [Theory, JSendAutoData]
        public void JsonSerializerSettingsCannotBeSetToNull(JSendApiController controller)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => controller.JsonSerializerSettings = null);
        }

        [Theory, JSendAutoData]
        public void JSendOkReturnsJSendOkResult(JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendOk();
            // Verify outcome
            result.Should().BeAssignableTo<JSendOkResult>();
        }

        [Theory, JSendAutoData]
        public void JSendOkWithContentReturnsJSendOkResult(Model model, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendOk(model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendOkResult<Model>>();
        }

        [Theory, JSendAutoData]
        public void JSendBadRequestReturnsJSendBadRequestResult(string reason, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendBadRequest(reason);
            // Verify outcome
            result.Should().BeAssignableTo<JSendBadRequestResult>();
        }

        [Theory, JSendAutoData]
        public void JSendBadRequestWithModelState_Returns_JSendInvalidModelStateResult(ModelStateDictionary modelState, JSendApiController controller)
        {
            // Fixture setup
            modelState.AddModelError("", "");
            // Exercise system
            var result = controller.JSendBadRequest(modelState);
            // Verify outcome
            result.Should().BeAssignableTo<JSendInvalidModelStateResult>();
        }

        [Theory, JSendAutoData]
        public void JSendCreatedWithUri_Returns_JSendCreatedResult(Uri uri, Model model, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendCreated(uri, model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendCreatedResult<Model>>();
        }

        [Theory, JSendAutoData]
        public void JSendCreatedWithString_Returns_JSendCreatedResult(Model model, JSendApiController controller)
        {
            // Fixture setup
            const string location = "http://localhost/";
            // Exercise system
            var result = controller.JSendCreated(location, model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendCreatedResult<Model>>();
        }

        [Theory, JSendAutoData]
        public void JSendCreatedWithString_Throws_IfLocationIsNull(Model model, JSendApiController controller)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => controller.JSendCreated(null as string, model));
        }

        [Theory, JSendAutoData]
        public void JSendCreatedWithString_AcceptsAbsoluteUris(Model model, JSendApiController controller)
        {
            // Fixture setup
            const string location = "http://localhost/";
            // Exercise system and verify outcome
            Assert.DoesNotThrow(() => controller.JSendCreated(location, model));
        }

        [Theory, JSendAutoData]
        public void JSendCreatedWithString_AcceptsRelativeUris(Model model, JSendApiController controller)
        {
            // Fixture setup
            const string location = "/about";
            // Exercise system and verify outcome
            Assert.DoesNotThrow(() => controller.JSendCreated(location, model));
        }

        [Theory, JSendAutoData]
        public void JSendCreatedAtRouteWithDictionary_ReturnsJSendCreatedAtRouteResult(IFixture fixture)
        {
            // Fixture setup
            const string routeLink = "http://localhost/models/";
            var urlFactoryMock = new Mock<UrlHelper>();
            urlFactoryMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                .Returns((string name, IDictionary<string, object> values) => routeLink);

            fixture.Inject(urlFactoryMock.Object);

            var routeName = fixture.Create<string>();
            var routeValues = fixture.Create<Dictionary<string, object>>();
            var model = fixture.Create<Model>();
            var controller = fixture.Create<JSendApiController>();
            // Exercise system
            var result = controller.JSendCreatedAtRoute(routeName, routeValues, model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendCreatedAtRouteResult<Model>>();
        }

        [Theory, JSendAutoData]
        public void JSendCreatedAtRouteWithObject_ReturnsJSendCreatedAtRouteResult(IFixture fixture)
        {
            // Fixture setup
            const string routeLink = "http://localhost/models/";
            var urlFactoryMock = new Mock<UrlHelper>();
            urlFactoryMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                .Returns((string name, IDictionary<string, object> values) => routeLink);

            fixture.Inject(urlFactoryMock.Object);

            var routeName = fixture.Create<string>();
            var routeValues = new {id = 5};
            var model = fixture.Create<Model>();
            var controller = fixture.Create<JSendApiController>();
            // Exercise system
            var result = controller.JSendCreatedAtRoute(routeName, routeValues, model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendCreatedAtRouteResult<Model>>();
        }
    }
}
