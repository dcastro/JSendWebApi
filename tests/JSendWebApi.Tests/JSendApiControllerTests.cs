using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.ModelBinding;
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

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (TestableJSendApiController).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void SettingsAndEncodingAreCorrectlyInitialized(JsonSerializerSettings settings, Encoding encoding)
        {
            // Exercise system
            var controller = new TestableJSendApiController(settings, encoding);
            // Verify outcome
            controller.JsonSerializerSettings.Should().BeSameAs(settings);
            controller.Encoding.Should().BeSameAs(encoding);
        }

        [Theory, JSendAutoData]
        public void SettingsAreCorrectlyInitialized(JsonSerializerSettings settings)
        {
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
        public void JsonSerializerSettingsCanBeSet(JsonSerializerSettings expectedSettings,
            JSendApiController controller)
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
        public void ReplacesExceptionHandler(TestableJSendApiController controller)
        {
            var context = new HttpControllerContext
            {
                Configuration = new HttpConfiguration()
            };
            var defaultHandler = context.Configuration.Services.GetService(typeof (IExceptionHandler));
            // Exercise system
            controller.TestableInitialize(context);
            // Verify outcome
            var handler = controller.Configuration.Services.GetService(typeof (IExceptionHandler));
            handler.Should().NotBe(defaultHandler);
            handler.Should().BeOfType<JSendExceptionHandler>();
        }

        [Theory, JSendAutoData]
        public void AddsValueActionFilter(TestableJSendApiController controller)
        {
            var context = new HttpControllerContext
            {
                Configuration = new HttpConfiguration()
            };
            // Exercise system
            controller.TestableInitialize(context);
            // Verify outcome
            controller.Configuration.Filters.Should().Contain(
                filter => filter.Instance.GetType() == typeof (ValueActionFilter));
        }

        [Theory, JSendAutoData]
        public void AddsVoidActionFilter(TestableJSendApiController controller)
        {
            var context = new HttpControllerContext
            {
                Configuration = new HttpConfiguration()
            };
            // Exercise system
            controller.TestableInitialize(context);
            // Verify outcome
            controller.Configuration.Filters.Should().Contain(
                filter => filter.Instance.GetType() == typeof (VoidActionFilter));
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
        public void JSendBadRequestWithModelState_Returns_JSendInvalidModelStateResult(ModelStateDictionary modelState,
            JSendApiController controller)
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
        public void JSendCreatedAtRouteWithDictionary_ReturnsJSendCreatedAtRouteResult(string routeName,
            Dictionary<string, object> routeValues, Model model, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendCreatedAtRoute(routeName, routeValues, model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendCreatedAtRouteResult<Model>>();
        }

        [Theory, JSendAutoData]
        public void JSendCreatedAtRouteWithObject_ReturnsJSendCreatedAtRouteResult(string routeName, Model model,
            JSendApiController controller)
        {
            // Fixture setup
            var routeValues = new {id = 5};
            // Exercise system
            var result = controller.JSendCreatedAtRoute(routeName, routeValues, model);
            // Verify outcome
            result.Should().BeAssignableTo<JSendCreatedAtRouteResult<Model>>();
        }

        [Theory, JSendAutoData]
        public void JSendInternalServerErrorWithMessage_ReturnsJSendInternalServerErrorResult(
            string message, int code, object data, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendInternalServerError(message, code, data);
            // Verify outcome
            result.Should().BeAssignableTo<JSendInternalServerErrorResult>();
        }

        [Theory, JSendAutoData]
        public void JSendInternalServerErrorWithException_ReturnsJSendExceptionResult(
            Exception ex, string message, int code, object data, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendInternalServerError(ex, message, code, data);
            // Verify outcome
            result.Should().BeAssignableTo<JSendExceptionResult>();
        }

        [Theory, JSendAutoData]
        public void JSendNotFoundWithReason_ReturnsJSendNotFoundResult(string reason, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendNotFound(reason);
            // Verify outcome
            result.Should().BeAssignableTo<JSendNotFoundResult>();
        }

        [Theory, JSendAutoData]
        public void JSendNotFound_ReturnsJSendNotFoundResult(JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendNotFound();
            // Verify outcome
            result.Should().BeAssignableTo<JSendNotFoundResult>();
        }

        [Theory, JSendAutoData]
        public void JSendRedirectWithUri_Returns_JSendRedirectResult(Uri uri, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendRedirect(uri);
            // Verify outcome
            result.Should().BeAssignableTo<JSendRedirectResult>();
        }

        [Theory, JSendAutoData]
        public void JSendRedirectWithString_Returns_JSendRedirectResult(JSendApiController controller)
        {
            // Fixture setup
            const string location = "http://localhost/";
            // Exercise system
            var result = controller.JSendRedirect(location);
            // Verify outcome
            result.Should().BeAssignableTo<JSendRedirectResult>();
        }

        [Theory, JSendAutoData]
        public void JSendRedirectToRouteWithDictionary_ReturnsJSendRedirectToRouteResult(string routeName,
            Dictionary<string, object> routeValues, JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSendRedirectToRoute(routeName, routeValues);
            // Verify outcome
            result.Should().BeAssignableTo<JSendRedirectToRouteResult>();
        }

        [Theory, JSendAutoData]
        public void JSendRedirectToRouteWithObject_ReturnsJSendRedirectToRouteResult(string routeName,
            JSendApiController controller)
        {
            // Fixture setup
            var routeValues = new {id = 5};
            // Exercise system
            var result = controller.JSendRedirectToRoute(routeName, routeValues);
            // Verify outcome
            result.Should().BeAssignableTo<JSendRedirectToRouteResult>();
        }

        [Theory, JSendAutoData]
        public void JSend_CreatesNewJSendResult(HttpStatusCode code, SuccessResponse response,
            JSendApiController controller)
        {
            // Exercise system
            var result = controller.JSend(code, response);
            // Verify outcome
            result.Response.Should().Be(response);
            result.StatusCode.Should().Be(code);
        }

        [Theory, JSendAutoData]
        public void JSendSuccess_CreatesNewJSendResult(HttpStatusCode code, Model model, JSendApiController controller)
        {
            // Fixture setup
            var expectedResponse = new SuccessResponse(model);
            // Exercise system
            var result = controller.JSendSuccess(code, model);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
            result.StatusCode.Should().Be(code);
        }

        [Theory, JSendAutoData]
        public void JSendFail_CreatesNewJSendResult(HttpStatusCode code, string reason, JSendApiController controller)
        {
            // Fixture setup
            var expectedResponse = new FailResponse(reason);
            // Exercise system
            var result = controller.JSendFail(code, reason);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
            result.StatusCode.Should().Be(code);
        }

        [Theory, JSendAutoData]
        public void JSendError_CreatesNewJSendError(HttpStatusCode code, string message, int? errorCode, string data,
            JSendApiController controller)
        {
            // Fixture setup
            var expectedResponse = new ErrorResponse(message, errorCode, data);
            // Exercise system
            var result = controller.JSendError(code, message, errorCode, data);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
            result.StatusCode.Should().Be(code);
        }
    }
}
