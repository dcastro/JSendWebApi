using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using FluentAssertions;
using JSend.WebApi.Responses;
using JSend.WebApi.Tests.FixtureCustomizations;
using JSend.WebApi.Tests.TestTypes;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace JSend.WebApi.Tests
{
    public class JSendValueResultConverterTests
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
        public void IsActionResultConverter(JSendValueResultConverter<Model> converter)
        {
            // Exercise system and verify outcome
            converter.Should().BeAssignableTo<IActionResultConverter>();
        }

        [Theory, JSendAutoData]
        public void ConstructorsThrowWhenAnyArgumentIsNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendValueResultConverter<Model>).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void ThrowsWhenControllerContextIsNull(Model value, JSendValueResultConverter<Model> converter)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => converter.Convert(null, value));
        }

        [Theory, JSendAutoData]
        public async Task ConvertReturnsSuccessMessage(IFixture fixture, Model model,
            JSendValueResultConverter<Model> converter)
        {
            // Fixture setup
            fixture.Customize<X509Certificate2>(c => c.OmitAutoProperties());

            var context = fixture.Create<HttpControllerContext>();
            var jsendSuccess = JsonConvert.SerializeObject(new SuccessResponse(model));
            // Exercise system
            var message = converter.Convert(context, model);
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }

        [Theory, JSendAutoData]
        public void StatusCodeIs200(IFixture fixture, Model model,
            JSendValueResultConverter<Model> converter)
        {
            // Fixture setup
            fixture.Customize<X509Certificate2>(c => c.OmitAutoProperties());
            var context = fixture.Create<HttpControllerContext>();
            // Exercise system
            var message = converter.Convert(context, model);
            // Verify outcome
            message.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory, JSendAutoData]
        public void SetsContentTypeHeader(IFixture fixture, Model model,
            JSendValueResultConverter<Model> converter)
        {
            // Fixture setup
            fixture.Customize<X509Certificate2>(c => c.OmitAutoProperties());
            var context = fixture.Create<HttpControllerContext>();
            // Exercise system
            var message = converter.Convert(context, model);
            // Verify outcome
            message.Content.Headers.ContentType.MediaType.Should().Be("application/json");
        }
    }
}
