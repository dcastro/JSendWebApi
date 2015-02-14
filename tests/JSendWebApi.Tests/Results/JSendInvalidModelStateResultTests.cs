using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FluentAssertions;
using JSendWebApi.Responses;
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
    public class JSendInvalidModelStateResultTests
    {
        private class InvalidModelCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<ModelStateDictionary>(c =>
                    c.Do(dic => dic.AddModelError("key", "value")));
            }
        }

        [Fact]
        public void IsHttpActionResult()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions()).Customize(new InvalidModelCustomization());
            var result = fixture.Create<JSendInvalidModelStateResult>();
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ThrowsIfModelStateIsValid(JSendApiController controller, ModelStateDictionary modelState)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentException>(() => new JSendInvalidModelStateResult(controller, modelState));
        }

        [Fact]
        public async Task ReturnFailJSendResponse()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions()).Customize(new InvalidModelCustomization());
            var result = fixture.Create<JSendInvalidModelStateResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Contain(@"""status"":""fail""");
        }

        [Fact]
        public async Task ExtractsErrorMessages()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions());
            fixture.Customize<ModelStateDictionary>(c =>
                c.Do(dic => dic.AddModelError("age", "error1"))
                    .Do(dic => dic.AddModelError("age", "error2")));

            var expectedErrorMessages = new JObject
            {
                {"age", new JArray("error1", "error2")}
            };
            var result = fixture.Create<JSendInvalidModelStateResult>();

            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());

            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            JToken.DeepEquals(jContent["data"], expectedErrorMessages).Should().BeTrue();
        }

        [Fact]
        public async Task ExtractsExceptionMessages()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions());
            fixture.Customize<ModelStateDictionary>(c =>
                c.Do(dic => dic.AddModelError("age", new Exception("exceptionMessage1")))
                    .Do(dic => dic.AddModelError("age", new Exception("exceptionMessage2"))));
            fixture.Freeze<JSendApiController>().RequestContext.IncludeErrorDetail = true;

            var expectedExceptionMessages = new JObject
            {
                {"age", new JArray("exceptionMessage1", "exceptionMessage2")}
            };
            var result = fixture.Create<JSendInvalidModelStateResult>();

            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());

            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            JToken.DeepEquals(jContent["data"], expectedExceptionMessages).Should().BeTrue();
        }

        [Fact]
        public async Task InsertsDefaultMessageInsteadOfExceptionMessage_If_ControllerIsConfuguredToNotIncludeErrorDetails()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions());
            fixture.Customize<ModelStateDictionary>(c =>
                c.Do(dic => dic.AddModelError("age", new Exception("exceptionMessage1"))));
            fixture.Freeze<JSendApiController>().RequestContext.IncludeErrorDetail = false;

            var result = fixture.Create<JSendInvalidModelStateResult>();

            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());

            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            var ageErrors = jContent["data"].Value<JArray>("age");

            ageErrors.Should().NotBeNull();
            ageErrors.Count.Should().Be(1);
            ageErrors.First.ToString().Should()
                .NotBe("exceptionMessage1")
                .And
                .NotBeEmpty();
        }

        [Fact]
        public async Task InsertsDefaultMessage_If_ErrorMessageIsEmpty()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions());
            fixture.Customize<ModelStateDictionary>(c =>
                c.Do(dic => dic.AddModelError("age", errorMessage: "")));

            var result = fixture.Create<JSendInvalidModelStateResult>();

            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());

            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            var jContent = JObject.Parse(content);
            var ageErrors = jContent["data"].Value<JArray>("age");

            ageErrors.Should().NotBeNull();
            ageErrors.Count.Should().Be(1);
            ageErrors.First.ToString().Should().NotBeEmpty();
        }

        [Fact]
        public async Task StatusCodeIs400()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions())
                .Customize(new InvalidModelCustomization());
            var result = fixture.Create<JSendInvalidModelStateResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SetsCharSetHeader()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions())
                .Customize(new InvalidModelCustomization());
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendInvalidModelStateResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet = encoding.WebName;
        }

        [Fact]
        public async Task SetsContentTypeHeader()
        {
            // Fixture setup
            var fixture = new Fixture().Customize(new JSendTestConventions())
                .Customize(new InvalidModelCustomization());

            var result = fixture.Create<JSendInvalidModelStateResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
