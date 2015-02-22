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
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(null, model));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenSerializerSettingsAreNull(Encoding encoding, HttpRequestMessage request,
            Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(null, encoding, request, model));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenEncodingIsNull(JsonSerializerSettings settings, HttpRequestMessage request,
            Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(settings, null, request, model));
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenRequestIsNull(JsonSerializerSettings settings, Encoding encoding, Model model)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JSendOkResult<Model>(settings, encoding, null, model));
        }


        [Theory, JSendAutoData]
        public void ResponseIsInitialized(JSendOkResult<Model> result)
        {
            // Exercise system and verify outcome
            result.Response.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public void ResponseIsSuccess(JSendOkResult<Model> result)
        {
            // Exercise system and verify outcome
            result.Response.Should().BeAssignableTo<SuccessJSendResponse>();
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
        public void ResponseDataIsCorrectlySet([Frozen] Model content, JSendOkResult<Model> result)
        {
            // Exercise system and verify outcome
            result.Response.Data.Should().BeSameAs(content);
        }

        [Theory, JSendAutoData]
        public async Task StatusCodeIs200(JSendOkResult<Model> result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.OK);
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
