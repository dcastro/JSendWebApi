﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSend.WebApi.Properties;
using JSend.WebApi.Responses;
using JSend.WebApi.Results;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.WebApi.Tests.Results
{
    public class JSendBadRequestResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendBadRequestResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendBadRequestResult).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ConstructorThrowsWhenReasonIsWhiteSpace(ApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendBadRequestResult("  ", controller);
            ctor.ShouldThrow<ArgumentException>()
                .And.Message.Should().StartWith(StringResources.BadRequest_WhiteSpaceReason);
        }

        [Theory, JSendAutoData]
        public void CanBeCreatedWithControllerWithoutProperties(string reason,
            [NoAutoProperties] TestableJSendApiController controller)
        {
            // Exercise system and verify outcome
            Action ctor = () => new JSendBadRequestResult(reason, controller);
            ctor.ShouldNotThrow();
        }

        [Theory, JSendAutoData]
        public void ResponseIsCorrectlyInitialized(string reason, ApiController controller)
        {
            // Fixture setup
            var expectedResponse = new FailResponse(reason);
            // Exercise system
            var result = new JSendBadRequestResult(reason, controller);
            // Verify outcome
            result.Response.ShouldBeEquivalentTo(expectedResponse);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs400(JSendBadRequestResult result)
        {
            // Exercise system and verify outcome
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory, JSendAutoData]
        public void RequestIsCorrectlyInitializedUsingController(string reason, ApiController controller)
        {
            // Exercise system
            var result = new JSendBadRequestResult(reason, controller);
            // Verify outcome
            result.Request.Should().Be(controller.Request);
        }

        [Theory, JSendAutoData]
        public void ReasonIsCorrectlyInitialized(string reason, ApiController controller)
        {
            // Exercise system
            var result = new JSendBadRequestResult(reason, controller);
            // Verify outcome
            result.Reason.Should().Be(reason);
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendBadRequestResult result)
        {
            // Fixture setup
            var serializedResponse = JsonConvert.SerializeObject(result.Response);
            // Exercise system
            var httpResponse = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await httpResponse.Content.ReadAsStringAsync();
            content.Should().Be(serializedResponse);
        }

        [Theory, JSendAutoData]
        public async Task SetsStatusCode(JSendBadRequestResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(result.StatusCode);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendBadRequestResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
