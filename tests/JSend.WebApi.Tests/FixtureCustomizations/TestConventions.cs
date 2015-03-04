using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace JSend.WebApi.Tests.FixtureCustomizations
{
    internal class TestConventions : CompositeCustomization
    {
        public TestConventions()
            : base(
                new AutoConfiguredMoqCustomization(),
                new JSendApiControllerCustomization(),
                new HttpRequestContextCustomization(),
                new JsonMediaTypeFormatterCustomization(),
                new UrlHelperCustomization(),
                new RandomHttpStatusCodeCustomization())
        {

        }
    }

    internal class JSendApiControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new TypeRelay(
                    typeof (JSendApiController),
                    typeof (TestableJSendApiController)));
        }
    }

    public class JsonMediaTypeFormatterCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<JsonMediaTypeFormatter>(
                c => c.OmitAutoProperties());
        }
    }

    internal class HttpRequestContextCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<HttpRequestContext>(
                c => c.Without(x => x.ClientCertificate));
        }
    }

    internal class UrlHelperCustomization : ICustomization
    {
        public const string RouteLink = "http://localhost/models/";

        public void Customize(IFixture fixture)
        {
            var urlFactoryMock = new Mock<UrlHelper>();
            urlFactoryMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<IDictionary<string, object>>()))
                .Returns((string name, IDictionary<string, object> values) => RouteLink);

            fixture.Inject(urlFactoryMock.Object);
        }
    }

    public class RandomHttpStatusCodeCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var rnd = new Random();
            fixture.Register<HttpStatusCode>(() => Generate(rnd));
        }

        private static HttpStatusCode Generate(Random rnd)
        {
            var values = Enum.GetValues(typeof(HttpStatusCode));
            return (HttpStatusCode)values.GetValue(rnd.Next(values.Length));
        }
    }
}
