using Ploeh.AutoFixture;

namespace JSendWebApi.Tests.FixtureCustomizations
{
    public class JSendAutoDataAttribute : Ploeh.AutoFixture.Xunit.AutoDataAttribute
    {
        public JSendAutoDataAttribute() : base(
            new Fixture().Customize(new JSendTestConventions()))
        {

        }
    }

    internal class JSendTestConventions : CompositeCustomization
    {
        public JSendTestConventions()
            : base(new JSendApiControllerCustomization())
        {

        }
    }
}
