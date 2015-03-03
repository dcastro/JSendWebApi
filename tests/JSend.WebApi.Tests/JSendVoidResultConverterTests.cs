using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
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
        public async Task ConvertReturnsSuccessMessage(IFixture fixture,
            JSendVoidResultConverter converter)
        {
            // Fixture setup
            fixture.Customize<X509Certificate2>(c => c.OmitAutoProperties());

            var context = fixture.Create<HttpControllerContext>();
            var jsendSuccess = JsonConvert.SerializeObject(new SuccessResponse());
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs200(IFixture fixture, JSendVoidResultConverter converter)
        {
            // Fixture setup
            fixture.Customize<X509Certificate2>(c => c.OmitAutoProperties());
            var context = fixture.Create<HttpControllerContext>();
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory, JSendAutoData]
        public void SetsContentTypeHeader(IFixture fixture, JSendVoidResultConverter converter)
        {
            // Fixture setup
            fixture.Customize<X509Certificate2>(c => c.OmitAutoProperties());
            var context = fixture.Create<HttpControllerContext>();
            // Exercise system
            var message = converter.Convert(context, null);
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
