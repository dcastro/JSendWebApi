using JSend.Client.Tests.FixtureCustomizations;
using Ploeh.AutoFixture.Idioms;
using Xunit;

namespace JSend.Client.Tests
{
    public class ResponseReceivedContextTests
    {
        [Theory, JSendAutoData]
        public void ArgumentsCannotBeNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (ResponseReceivedContext).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void PropertiesAreCorrectlyInitialized(ConstructorInitializedMemberAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (ResponseReceivedContext).GetProperties());
        }
    }
}
