using FluentAssertions;
using JSend.Client.Responses;
using JSend.Client.Tests.FixtureCustomizations;
using Ploeh.Albedo;
using Ploeh.AutoFixture.Idioms;
using Xunit;

namespace JSend.Client.Tests.Responses
{
    public class JSuccessWithDataResponseTests
    {
        [Theory, JSendAutoData]
        public void PropertiesAreCorrectlyInitialized(ConstructorInitializedMemberAssertion assertion)
        {
            // Fixture setup
            var property = new Properties<JSuccessWithDataResponse<int>>();
            // Exercise system and verify outcome
            assertion.Verify(property.Select(r => r.Data));
            assertion.Verify(property.Select(r => r.HttpResponse));
        }

        [Theory, JSendAutoData]
        public void ErrorIsNull(JSuccessWithDataResponse<int> response)
        {
            // Exercise system and verify outcome
            response.Error.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public void HasData(JSuccessWithDataResponse<int> response)
        {
            // Exercise system and verify outcome
            response.HasData.Should().BeTrue();
        }

        [Theory, JSendAutoData]
        public void StatusIsSuccess(JSuccessWithDataResponse<int> response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be(JSendStatus.Success);
        }
    }
}
