using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace JSend.Client.Tests.FixtureCustomizations
{
    internal class TestConventions : CompositeCustomization
    {
        public TestConventions() :
            base(
            new AutoConfiguredMoqCustomization(),
            new JTokenCustomization(),
            new JSendErrorCustomization(),
            new EncodingCustomization(),
            new JsonSerializerSettingsCustomization())
        {

        }
    }

    internal class JTokenCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new TypeRelay(
                    typeof (JToken),
                    typeof (JObject)));
        }
    }

    internal class JSendErrorCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<JSendError>(
                c => c.FromFactory(
                    (string msg, int? code, JToken data) => new JSendError(JSendStatus.Fail, msg, code, data)));
        }
    }

    internal class EncodingCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            // Setting Encoding.EncoderFallback throws an exception,
            // so we'll just use a default encoding
            fixture.Inject<Encoding>(Encoding.UTF8);
        }
    }

    internal class JsonSerializerSettingsCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            // Setting JsonSerializerSettings.Culture throws an exception,
            // so we'll just use the default settings
            fixture.Customize<JsonSerializerSettings>(
                c => c.OmitAutoProperties());
        }
    }
}
