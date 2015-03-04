using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class JSendVoidResultConverterTests
    {
        [Theory, JSendAutoData]
        public void IsActionResultConverter(JSendVoidResultConverter converter)
        {
            // Exercise system and verify outcome
            converter.Should().BeAssignableTo<IActionResultConverter>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendVoidResultConverter).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenControllerContextIsNull(JSendVoidResultConverter converter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => converter.Convert(null, null));
        }

        [Theory, JSendAutoData]
        public async Task ConvertReturnsSuccessMessage(HttpControllerContext context,
            JSendVoidResultConverter converter)
        {
            // Fixture setup
            var jsendSuccess = JsonConvert.SerializeObject(new SuccessResponse());
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs200(HttpControllerContext context, JSendVoidResultConverter converter)
        {
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory, JSendAutoData]
        public void SetsContentTypeHeader(HttpControllerContext context, JSendVoidResultConverter converter)
        {
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
