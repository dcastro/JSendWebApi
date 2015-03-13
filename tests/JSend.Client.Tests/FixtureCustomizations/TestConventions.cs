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
            new JSendErrorCustomization())
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
}
