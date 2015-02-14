using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace JSendWebApi.Tests.FixtureCustomizations
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
                    .With(a => a.Request)
                    .With(a => a.JsonSerializerSettings)
                    .With(a => a.Encoding));
        }
    }
}
