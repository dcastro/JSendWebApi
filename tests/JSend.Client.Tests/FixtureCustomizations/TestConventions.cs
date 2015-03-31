using System.Net.Http;
using System.Text;
using JSend.Client.Tests.TestTypes;
using Moq;
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
            new JsonSerializerSettingsCustomization(),
            new JSendParserCustomization())
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

    internal class JSendParserCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            // Due to a limitation in Moq and AutoConfiguredMoqCustomization, generic methods are not automatically configured.
            // Therefore, we set them up manually, aided by ReturnsUsingFixture.
            // See https://github.com/AutoFixture/AutoFixture/wiki/Cheat-Sheet#more-information-16
            var parserMock = fixture.Freeze<Mock<IJSendParser>>();

            SetupParseAsync<Model>(parserMock, fixture);
            SetupParseAsync<JToken>(parserMock, fixture);
            SetupParseAsync<object>(parserMock, fixture);
        }

        private static void SetupParseAsync<T>(Mock<IJSendParser> parserMock, IFixture fixture)
        {
            parserMock
                .Setup(parser => parser.ParseAsync<T>(It.IsAny<HttpResponseMessage>()))
                .ReturnsUsingFixture(fixture);
        }
    }
}
