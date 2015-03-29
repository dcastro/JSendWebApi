using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.FunctionalTests.FixtureCustomizations;
using Xunit.Extensions;
using Newtonsoft.Json.Linq;

namespace JSend.Client.FunctionalTests
{
    public class ResponseParsingTests
    {
        [Theory, JSendAutoData]
        public async Task ParsesSuccessResponseWithoutData(JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.GetAsync<User>("http://localhost/users/success"))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Success);
                response.HasData.Should().BeFalse();
            }
        }

        [Theory, JSendAutoData]
        public async Task ParsesSuccessResponseWithData(JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.GetAsync<User>("http://localhost/users/success-with-user"))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Success);
                response.HasData.Should().BeTrue();
                response.Data.ShouldBeEquivalentTo(UsersController.TestUser);
            }
        }

        [Theory, JSendAutoData]
        public async Task ParsesFailResponse(JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.GetAsync<User>("http://localhost/users/fail"))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Fail);
                response.HasData.Should().BeFalse();
                response.Error.Data.Value<string>().Should().Be(UsersController.ErrorData);
            }
        }

        [Theory, JSendAutoData]
        public async Task ParsesErrorResponse(JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.GetAsync<User>("http://localhost/users/error"))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Error);
                response.HasData.Should().BeFalse();
                response.Error.Message.Should().Be(UsersController.ErrorMessage);
                response.Error.Code.Should().Be(UsersController.ErrorCode);
                response.Error.Data.Value<string>().Should().Be(UsersController.ErrorData);
            }
        }

        [Theory, JSendAutoData]
        public void DoesNotParseEmptyResponse(JSendClient client)
        {
            using (client)
            {
                // Exercise system and verify outcome
                client.Awaiting(c => c.GetAsync<User>("http://localhost/users/no-content"))
                    .ShouldThrow<JSendParseException>();
            }
        }
    }
}
