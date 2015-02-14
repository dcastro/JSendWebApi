using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Ploeh.AutoFixture;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    internal class JSendApiControllerCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<JSendApiController>(
                c => c.FromFactory(() => new TestableJSendApiController())
                    .OmitAutoProperties()
                    .With(a => a.Request, new HttpRequestMessage())
                    .With(a => a.JsonSerializerSettings, new JsonSerializerSettings())
                    .With(a => a.Encoding, new UTF8Encoding()));
        }
    }
}
