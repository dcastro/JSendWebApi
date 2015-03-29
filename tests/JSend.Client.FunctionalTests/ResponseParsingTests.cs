using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.FunctionalTests.FixtureCustomizations;
using Xunit.Extensions;

namespace JSend.Client.FunctionalTests
{
    public class ResponseParsingTests
    {
        [Theory, JSendAutoData]
        public async Task ParsesSuccessResponseWithoutData(JSendClient client)
        {
            // Exercise system
            using (var response = await client.GetAsync<User>("http://localhost/users/success"))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Success);
                response.HasData.Should().BeFalse();
            }
        }
    }
}
