using Ploeh.AutoFixture;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    public class JSendAutoDataAttribute : Ploeh.AutoFixture.Xunit.AutoDataAttribute
    {
        public JSendAutoDataAttribute() : base(
            new Fixture().Customize(new TestConventions()))
        {

        }
    }
}
