using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace JSend.Client.Tests.FixtureCustomizations
{
    internal class PropertyJSendAutoDataAttribute : CompositeDataAttribute
    {
        public PropertyJSendAutoDataAttribute(string propertyName)
            : base(
                new MemberDataAttribute(propertyName),
                new JSendAutoDataAttribute())
        {
        }
    }
}
