using Ploeh.AutoFixture.Xunit2;

namespace JSend.WebApi.FunctionalTests.FixtureCustomizations
{
    public class InlineJSendAutoDataAttribute : InlineAutoDataAttribute
    {
        public InlineJSendAutoDataAttribute(params object[] values)
            : base(new JSendAutoDataAttribute(), values)
        {

        }
    }
}
