using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace JSend.WebApi.FunctionalTests.FixtureCustomizations
{
    public class JSendAutoDataAttribute : AutoDataAttribute
    {
        public JSendAutoDataAttribute()
            : base(
                new Fixture().Customize(new TestConventions()))
        {

        }
    }
}
