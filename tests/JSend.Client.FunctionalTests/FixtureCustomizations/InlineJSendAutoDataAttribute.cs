using Ploeh.AutoFixture.Xunit2;

namespace JSend.Client.FunctionalTests.FixtureCustomizations
{
    public class InlineJSendAutoDataAttribute : InlineAutoDataAttribute
    {
        public InlineJSendAutoDataAttribute(params object[] values)
            : base(new JSendAutoDataAttribute(), values)
        {

        }
    }
}
