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
                    .With(a => a.Request));
        }
    }
}
