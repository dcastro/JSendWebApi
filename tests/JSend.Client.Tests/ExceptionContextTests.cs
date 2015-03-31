using JSend.Client.Tests.FixtureCustomizations;
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace JSend.Client.Tests
{
    public class ExceptionContextTests
    {
        [Theory, JSendAutoData]
        public void ArgumentsCannotBeNull(GuardClauseAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (ExceptionContext).GetConstructors());
        }

        [Theory, JSendAutoData]
        public void PropertiesAreCorrectlyInitialized(ConstructorInitializedMemberAssertion assertion)
        {
            // Exercise system and verify outcome
            assertion.Verify(typeof (ExceptionContext).GetProperties());
        }
    }
}
