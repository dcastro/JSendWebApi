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
    public class GenericJSendOkResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendOkResult<Model> result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenControllerIsNull(Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, null));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenSerializerSettingsAreNull(Encoding encoding, HttpRequestMessage request,
            Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, null, encoding, request));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenEncodingIsNull(JsonSerializerSettings settings, HttpRequestMessage request,
            Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, settings, null, request));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenRequestIsNull(JsonSerializerSettings settings, Encoding encoding, Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(model, settings, encoding, null));
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(JSendApiController controller, Model content)
        {
            // Fixture setup
            var expectedResponse = new SuccessResponse(content);
            // Exercise system
            var result = new JSendOkResult<Model>(content, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs200(JSendOkResult<Model> result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory, JSendAutoData]
        public void ContentIsCorrectlyInitialized(JSendApiController controller, Model content)
        {
            // Exercise system
            var result = new JSendOkResult<Model>(content, controller);
            // Verify outcome
            result.Content.Should().Be(content);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendOkResult<Model> result)
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
        public async Task SetsStatusCode(JSendOkResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendOkResult<Model>>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendOkResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
