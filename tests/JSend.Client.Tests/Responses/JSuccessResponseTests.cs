using FluentAssertions;
using JSend.Client.Properties;
using JSend.Client.Responses;
using JSend.Client.Tests.FixtureCustomizations;
using Ploeh.Albedo;
using Ploeh.AutoFixture.Idioms;
using Xunit;

namespace JSend.Client.Tests.Responses
{
    public class JSuccessResponseTests
    {
        [Theory, JSendAutoData]
        public void HttpResponseIsCorrectlyInitialized(ConstructorInitializedMemberAssertion assertion)
        {
            // Fixture setup
            var property = new Properties<JSuccessResponse<int>>();
            // Exercise system and verify outcome
            assertion.Verify(property.Select(r => r.HttpResponse));
        }

        [Theory, JSendAutoData]
        public void ErrorIsNull(JSuccessResponse<int> response)
        {
            // Exercise system and verify outcome
            response.Error.Should().BeNull();
        }

        [Theory, JSendAutoData]
        public void DoesNotHaveData(JSuccessResponse<int> response)
        {
            // Exercise system and verify outcome
            response.HasData.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void StatusIsSuccess(JSuccessResponse<int> response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be(JSendStatus.Success);
        }

        [Theory, JSendAutoData]
        public void DataThrowsException(JSuccessResponse<int> response)
        {
            // Exercise system and verify outcome
            response.Invoking(r => { var x = r.Data; })
                .ShouldThrow<JSendRequestException>()
                .WithMessage(StringResources.SuccessResponseWithoutData);
        }
    }
}
