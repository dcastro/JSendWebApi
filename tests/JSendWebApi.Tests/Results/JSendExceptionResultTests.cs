using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Results;
using JSendWebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Results
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
            Assert.Throws<ArgumentNullException>(() => new JSendExceptionResult(null, ex, message, code, data));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenExceptionIsNull(
            JSendApiController controller, string message, int? code, object data)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(
                () => new JSendExceptionResult(controller, null, message, code, data));
        }

        [Theory, JSendAutoData]
        public async Task ReturnsErrorJSendResponse(JSendExceptionResult result)
        {
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            content.Should().Contain(@"""status"":""error""");
        }

        [Theory, JSendAutoData]
        public async Task SerializesMessageIfNotNull([Frozen] string message, JSendExceptionResult result)
        {
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            var messageField = jContent.Value<string>("message");
            messageField.Should().Be(message);
        }

        [Theory, JSendAutoData]
        public async Task SerializesExceptionMessage_If_MessageIsNull_And_ControllerIsConfuguredToIncludeErrorDetails(
            JSendApiController controller, Exception ex, int? code, object data)
        {
            // Fixture setup
            controller.RequestContext.IncludeErrorDetail = true;
            var result = new JSendExceptionResult(controller, ex, null, code, data);
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            var messageField = jContent.Value<string>("message");
            messageField.Should().Be(ex.Message);
        }

        [Theory, JSendAutoData]
        public async Task SerializesDefaultMessage_If_MessageIsNull_And_ControllerIsConfuguredToNotIncludeErrorDetails(
            JSendApiController controller, Exception ex, int? code, object data)
        {
            // Fixture setup
            controller.RequestContext.IncludeErrorDetail = false;
            var result = new JSendExceptionResult(controller, ex, null, code, data);
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            var messageField = jContent.Value<string>("message");
            messageField.Should().Be("An error has occurred.");
        }

        [Theory, JSendAutoData]
        public async Task SerializesDataIfNotNull(JSendApiController controller, Exception ex, string message, int? code)
        {
            // Fixture setup
            const string data = "some string";
            var result = new JSendExceptionResult(controller, ex, message, code, data);
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            var dataField = jContent.Value<string>("data");
            dataField.Should().Be(data);
        }

        [Theory, JSendAutoData]
        public async Task SerializesException_If_DataIsNull_And_ControllerIsConfuguredToIncludeErrorDetails(
            JSendApiController controller, Exception ex, string message, int? code)
        {
            // Fixture setup
            controller.RequestContext.IncludeErrorDetail = true;
            var result = new JSendExceptionResult(controller, ex, message, code, null);
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            var dataField = jContent.Value<string>("data");
            dataField.Should().Be(ex.ToString());
        }

        [Theory, JSendAutoData]
        public async Task DoesNotSerializeDataField_If_DataIsNull_And_ControllerIsConfuguredToNotIncludeErrorDetails(
            JSendApiController controller, Exception ex, string message, int? code)
        {
            // Fixture setup
            controller.RequestContext.IncludeErrorDetail = false;
            var result = new JSendExceptionResult(controller, ex, message, code, null);
            // Exercise system
            var httpResponseMessage = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            jContent.Should().NotContainKey("data");
        }

        [Theory, JSendAutoData]
        public async Task StatusCodeIs500(JSendExceptionResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Theory, JSendAutoData]
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendExceptionResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
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
