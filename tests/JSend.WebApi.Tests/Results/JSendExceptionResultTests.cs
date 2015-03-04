using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests.Results
{
    public class JSendExceptionResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendExceptionResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(Exception ex, string message, int? code, object data)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendExceptionResult(ex, message, code, data, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenExceptionIsNull(
            string message, int? code, object data, bool includeErrorDetail, JsonMediaTypeFormatter formatter,
            HttpRequestMessage request, ApiController controller)
        {
            // Fixture setup
            Exception ex = null;
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendExceptionResult(ex, message, code, data, includeErrorDetail, formatter, request));
            Assert.Throws<ArgumentNullException>(() => new JSendExceptionResult(ex, message, code, data, controller));
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs500(JSendExceptionResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Theory, JSendAutoData]
        public void ExceptionIsCorrectlyInitialized(Exception ex, string message, int? code, object data,
            JSendApiController controller)
        {
            // Exercise system
            var result = new JSendExceptionResult(ex, message, code, data, controller);
            // Verify outcome
            result.Exception.Should().Be(ex);
        }

        [Theory, JSendAutoData]
        public void ResponseIsError(JSendExceptionResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().BeAssignableTo<ErrorResponse>();
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendExceptionResult result)
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
        public void ResponseMessageIsSetToMessage_When_MessageIsNotNull(
            Exception ex, string message, int? code, object data, JSendApiController controller)
        {
            // Exercise system
            var result = new JSendExceptionResult(ex, message, code, data, controller);
            // Verify outcome
            result.Response.Message.Should().Be(message);
        }

        [Theory, JSendAutoData]
        public void ResponseMessageIsSetToExceptionMessage_When_MessageIsNull_And_ControllerIsConfiguredToIncludeErrorDetails(
            Exception ex, int? code, object data, JSendApiController controller)
        {
            // Fixture setup
            controller.RequestContext.IncludeErrorDetail = true;
            // Exercise system
            var result = new JSendExceptionResult(ex, null, code, data, controller);
            // Verify outcome
            result.Response.Message.Should().Be(ex.Message);
        }

        [Theory, JSendAutoData]
        public void ResponseMessageIsSetToDefaultMessage_When_MessageIsNull_And_ControllerIsConfiguredToNotIncludeErrorDetails(
            Exception ex, int? code, object data, JSendApiController controller)
        {
            // Fixture setup
            controller.RequestContext.IncludeErrorDetail = false;
            // Exercise system
            var result = new JSendExceptionResult(ex, null, code, data, controller);
            // Verify outcome
            result.Response.Message.Should().Be(StringResources.DefaultErrorMessage);
        }

        [Theory, JSendAutoData]
        public void ResponseDataIsSetToData_When_DataIsNotNull(
            Exception ex, string message, int? code, object data, JSendApiController controller)
        {
            // Exercise system 
            var result = new JSendExceptionResult(ex, message, code, data, controller);
            // Verify outcome
            result.Response.Data.Should().BeSameAs(data);
        }

        [Theory, JSendAutoData]
        public void ResponseDataIsSetToStringifiedException_When_DataIsNull_And_ControllerIsConfiguredToIncludeErrorDetails(
            Exception ex, string message, int? code, JSendApiController controller)
        {
            // Fixture setup
            controller.RequestContext.IncludeErrorDetail = true;
            // Exercise system
            var result = new JSendExceptionResult(ex, message, code, null, controller);
            // Verify outcome
            result.Response.Data.Should().Be(ex.ToString());
        }

        [Theory, JSendAutoData]
        public void ResponseDataIsSetToNull_When_DataIsNull_And_ControllerIsConfiguredToNotIncludeErrorDetails(
            Exception ex, string message, int? code, JSendApiController controller)
        {
            // Fixture setup
            controller.RequestContext.IncludeErrorDetail = false;
            // Exercise system
            var result = new JSendExceptionResult(ex, message, code, null, controller);
            // Verify outcome
            result.Response.Data.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public void ResponseCodeIsCorrectlySet([Frozen] int? code, JSendExceptionResult result)
        {
            // Exercise system and verify outcome
            result.Response.Code.Should().HaveValue()
                .And.Be(code);
        }

        [Theory, JSendAutoData]
        public async Task SetsStatusCode(JSendExceptionResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendExceptionResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
