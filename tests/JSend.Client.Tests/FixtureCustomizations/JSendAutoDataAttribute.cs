using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace JSend.Client.Tests.FixtureCustomizations
{
    public class JSendAutoDataAttribute : AutoDataAttribute
    {
        public JSendAutoDataAttribute() : base(
            new Fixture().Customize(new TestConventions()))
        {

        }
    }
}
