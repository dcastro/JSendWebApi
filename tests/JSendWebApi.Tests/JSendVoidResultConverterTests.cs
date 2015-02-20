using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSendWebApi.Tests
{
    public class JSendVoidResultConverterTests
    {
        private class HttpControllerContextCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<HttpControllerContext>(
                    c => c.OmitAutoProperties()
                        .With(ctx => ctx.Request));
            }
        }

        private class WithRequestAttribute : CustomizeAttribute
        {
            public override ICustomization GetCustomization(ParameterInfo parameter)
            {
                return new HttpControllerContextCustomization();
            }
        }

        [Theory, JSendAutoData]
        public void IsActionResultConverter(JSendVoidResultConverter converter)
        {
            // Exercise system and verify outcome
            converter.Should().BeAssignableTo<IActionResultConverter>();
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenControllerContextIsNull(JSendVoidResultConverter converter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => converter.Convert(null, null));
        }

        [Theory, JSendAutoData]
        public async Task ConvertReturnsSuccessJSendMessage([WithRequest] HttpControllerContext context,
            JSendVoidResultConverter converter)
        {
            // Fixture setup
            var jsendSuccess = JsonConvert.SerializeObject(new SuccessJSendResponse());
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs200([WithRequest] HttpControllerContext context, JSendVoidResultConverter converter)
        {
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory, JSendAutoData]
        public void SetsCharSetHeader(IFixture fixture, [WithRequest] HttpControllerContext context)
        {
            // Fixture setup
            var encoding = Encoding.ASCII;
            fixture.Inject(encoding);

            var converter = fixture.Create<JSendVoidResultConverter>();
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            message.Content.Headers.ContentType.CharSet.Should().Be(encoding.WebName);
        }

        [Theory, JSendAutoData]
        public void SetsContentTypeHeader([WithRequest] HttpControllerContext context,
            JSendVoidResultConverter converter)
        {
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
