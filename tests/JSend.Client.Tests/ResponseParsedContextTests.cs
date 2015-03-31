using JSend.Client.Tests.FixtureCustomizations;
using JSend.Client.Tests.TestTypes;
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class ResponseParsedContextTests
    {
        [Theory, JSendAutoData]
        public void ArgumentsCannotBeNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (ResponseParsedContext<Model>).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void PropertiesAreCorrectlyInitialized(ConstructorInitializedMemberAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (ResponseParsedContext<Model>).GetProperties());
        }
    }
}
