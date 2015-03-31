using Ploeh.AutoFixture.Xunit;

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
