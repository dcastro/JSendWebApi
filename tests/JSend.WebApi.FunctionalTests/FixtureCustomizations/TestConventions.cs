using System.Net.Http;
using System.Web.Http;
using Ploeh.AutoFixture;

namespace JSend.WebApi.FunctionalTests.FixtureCustomizations
{
    internal class TestConventions : CompositeCustomization
    {
        public TestConventions()
            : base(
                new HttpConfigurationCustomization(),
                new HttpServerCustomization(),
                new HttpClientCustomization())
        {

        }

        private class HttpConfigurationCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<HttpConfiguration>(
                    c => c.OmitAutoProperties()
                        .Do(config => config.MapHttpAttributeRoutes()));
            }
        }

        private class HttpServerCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var config = fixture.Create<HttpConfiguration>();
                fixture.Inject(new HttpServer(config));
            }
        }

        private class HttpClientCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                var server = fixture.Create<HttpServer>();
                fixture.Inject(new HttpClient(server));
            }
        }
    }
}
