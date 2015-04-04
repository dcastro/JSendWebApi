using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace JSend.Client.Tests.FixtureCustomizations
{
    internal class PropertyJSendAutoDataAttribute : CompositeDataAttribute
    {
        public PropertyJSendAutoDataAttribute(string propertyName)
            : base(
                new PropertyDataAttribute(propertyName),
                new JSendAutoDataAttribute())
        {
        }
    }
}
