using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using JSend.WebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class JSendValueResultConverterTests
    {
        [Theory, JSendAutoData]
        public void IsActionResultConverter(JSendValueResultConverter<Model> converter)
        {
            // Exercise system and verify outcome
            converter.Should().BeAssignableTo<IActionResultConverter>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenControllerContextIsNull(Model value, JSendValueResultConverter<Model> converter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => converter.Convert(null, value));
        }

        [Theory, JSendAutoData]
        public async Task ConvertReturnsSuccessMessage(HttpControllerContext context, Model model,
            JSendValueResultConverter<Model> converter)
        {
            // Fixture setup
            var jsendSuccess = JsonConvert.SerializeObject(new SuccessResponse(model));
            // Exercise system
            var message = converter.Convert(context, model);
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs200(HttpControllerContext context, Model model,
            JSendValueResultConverter<Model> converter)
        {
            // Exercise system
            var message = converter.Convert(context, model);
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory, JSendAutoData]
        public void SetsContentTypeHeader(HttpControllerContext context, Model model,
            JSendValueResultConverter<Model> converter)
        {
            // Exercise system
            var message = converter.Convert(context, model);
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
