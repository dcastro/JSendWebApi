using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Results;
using JSendWebApi.Tests.TestClasses;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Kernel;
using Xunit;

namespace JSendWebApi.Tests
{
    public class JSendApiControllerTests
    {
        [Fact]
        public void JSendApiControllerIsApiController()
        {
            // Fixture setup
            // Exercise system
            var controller = new TestableJSendApiController();
            // Verify outcome
            controller.Should().BeAssignableTo<ApiController>();
        }

        [Fact]
        public void JSendOkReturnsJSendOkResult()
        {
            // Fixture setup
            var controller = new TestableJSendApiController();
            // Exercise system
            var result = controller.JSendOk();
            // Verify outcome
            result.Should().BeAssignableTo<JSendOkResult>();
        }

        [Fact]
        public void JSendOkWithContentReturnsJSendOkResult()
        {
            // Fixture setup
            var controller = new TestableJSendApiController();
            var model = new Model();
            // Exercise system
            var result = controller.JSendOk(model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendOkResult<Model>>();
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
        public void ConstructorsThrowsWhenAnyArgumentIsNull()
        {
            // Fixture setup
            var fixture = new Fixture {OmitAutoProperties = true};
            var assertion = new GuardClauseAssertion(fixture);
            // Exercise system and verify outcome
            assertion.Verify(typeof (TestableJSendApiController).GetConstructors());
        }
    }
}
