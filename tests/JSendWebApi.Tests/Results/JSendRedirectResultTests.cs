﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Results;
using JSendWebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests.Results
{
    public class JSendRedirectResultTests
    {
        [Theory, JSendAutoData]
        public void IsHttpActionResult(JSendRedirectResult result)
        {
            // Exercise system and verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendRedirectResult).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ResponseIsInitialized(JSendRedirectResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().NotBeNull();
        }

        [Theory, JSendAutoData]
        public void ResponseIsSuccess(JSendRedirectResult result)
        {
            // Exercise system and verify outcome
            result.Response.Should().BeAssignableTo<SuccessJSendResponse>();
        }

        [Theory, JSendAutoData]
        public async Task ResponseIsSerializedIntoBody(JSendRedirectResult result)
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
        public void ResponseDataIsNull(JSendRedirectResult result)
        {
            // Exercise system and verify outcome
            result.Response.Data.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public async Task StatusCodeIs302(JSendRedirectResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Theory, JSendAutoData]
        public async Task SetsCharSetHeader(IFixture fixture)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var result = fixture.Create<JSendRedirectResult>();
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
        }

        [Theory, JSendAutoData]
        public async Task SetsContentTypeHeader(JSendRedirectResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }

        [Theory, JSendAutoData]
        public async Task SetsLocationHeader([Frozen] Uri expectedLocation, JSendRedirectResult result)
        {
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            message.Headers.Location.Should().Be(expectedLocation);
        }
    }
}
