using System.Net.Http;
using System.Text;
using System.Web.Http;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace JSend.WebApi.Tests.FixtureCustomizations
{
    internal class JSendApiControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new TypeRelay(
                    typeof (JSendApiController),
                    typeof (TestableJSendApiController)));

            fixture.Customize<TestableJSendApiController>(
                c => c.OmitAutoProperties()
                    .With(x => x.Request)
                    .With(x => x.Url)
                    .With(x => x.Configuration));

            fixture.Customize<HttpConfiguration>(
                c => c.Without(x => x.DependencyResolver));

            fixture.Customize<HttpRequestMessage>(
                c => c.OmitAutoProperties());
        }
    }
}
