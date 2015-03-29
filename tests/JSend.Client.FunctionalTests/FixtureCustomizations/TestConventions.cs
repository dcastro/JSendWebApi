using System.Net.Http;
using System.Web.Http;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace JSend.Client.FunctionalTests.FixtureCustomizations
{
    internal class TestConventions : CompositeCustomization
    {
        public TestConventions()
            : base(
                new HttpConfigurationCustomization(),
                new HttpServerCustomization(),
                new HttpClientCustomization(),
                new JSendClientCustomization(),
                new JSendClientSettingsCustomization())
        {

        }

        private class HttpConfigurationCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<HttpConfiguration>(
                    c => c
                        .OmitAutoProperties()
                        .Do(config => config.MapHttpAttributeRoutes()));
            }
        }

        private class HttpServerCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Register((HttpConfiguration config) => new HttpServer(config));
            }
        }

        private class HttpClientCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Register((HttpServer server) => new HttpClient(server));
            }
        }

        private class JSendClientCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<JSendClient>(
                    c => c.FromFactory(new MethodInvoker(
                        new GreedyConstructorQuery())));
            }
        }

        private class JSendClientSettingsCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<JSendClientSettings>(
                    c => c.OmitAutoProperties());
            }
        }
    }
}
