using JSend.Client.Tests.FixtureCustomizations;
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class JSendClientSettingsTests
    {
        [Theory, JSendAutoData]
        public void WritablePropertiesBehaveCorrectly(WritablePropertyAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (JSendClientSettings).GetProperties());
        }
    }
}
