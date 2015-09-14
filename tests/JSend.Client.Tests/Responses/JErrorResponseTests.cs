using System;
using System.Net.Http;
using FluentAssertions;
using JSend.Client.Responses;
using JSend.Client.Tests.FixtureCustomizations;
using Ploeh.Albedo;
using Ploeh.AutoFixture.Idioms;
using Xunit;

namespace JSend.Client.Tests.Responses
{
    public class JErrorResponseTests
    {
        [Theory, JSendAutoData]
        public void ErrorMustNotBeNull(HttpResponseMessage httpResponse)
        {
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => new JErrorResponse<int>(null, httpResponse));
        }

        [Theory, JSendAutoData]
        public void PropertiesAreCorrectlyInitializer(ConstructorInitializedMemberAssertion assertion)
        {
            // Fixture setup
            var properties = new Properties<JErrorResponse<int>>();
            // Exercise system and verify outcome
            assertion.Verify(properties.Select(r => r.Error));
            assertion.Verify(properties.Select(r => r.HttpResponse));
        }

        [Theory, JSendAutoData]
        public void DoesNotHaveData(JErrorResponse<int> response)
        {
            // Exercise system and verify outcome
            response.HasData.Should().BeFalse();
        }

        [Theory, JSendAutoData]
        public void StatusMatchesErrorStatus(JErrorResponse<int> response)
        {
            // Exercise system and verify outcome
            response.Status.Should().Be(response.Error.Status);
        }

        [Theory, JSendAutoData]
        public void DataThrowsException(JErrorResponse<int> response)
        {
            // Exercise system and verify outcome
            response.Invoking(r => { var x = r.Data; })
                .ShouldThrow<JSendRequestException>()
                .WithMessage("JSend status does not indicate success: \"fail\".");
        }
    }
}
