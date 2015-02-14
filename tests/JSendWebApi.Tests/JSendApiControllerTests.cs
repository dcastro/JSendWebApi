using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FluentAssertions;
using JSendWebApi.Results;
using JSendWebApi.Tests.FixtureCustomizations;
using JSendWebApi.Tests.TestTypes;
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
            result.Should().BeAssignableTo<JSendBadRequestResult<string>>();
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
    }
}
