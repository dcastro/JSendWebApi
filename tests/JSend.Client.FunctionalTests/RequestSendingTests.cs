using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.FunctionalTests.FixtureCustomizations;
using Xunit.Extensions;

namespace JSend.Client.FunctionalTests
{
    public class RequestSendingTests
    {
        [Theory, JSendAutoData]
        public async Task SendsGetRequestsToTheCorrectEndpoint(JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.GetAsync<string>("http://localhost/users/get"))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Success);
                response.Data.Should().Be("get");
            }
        }

        [Theory, JSendAutoData]
        public async Task SendsPostRequestsToTheCorrectEndpoint(User user, JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.PostAsync<string>("http://localhost/users/post", user))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Success);
                response.Data.Should().Be("post");
            }
        }

        [Theory, JSendAutoData]
        public async Task SendsPutRequestsToTheCorrectEndpoint(User user, JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.PutAsync<string>("http://localhost/users/put", user))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Success);
                response.Data.Should().Be("put");
            }
        }

        [Theory, JSendAutoData]
        public async Task SendsDeleteRequestsToTheCorrectEndpoint(JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.DeleteAsync("http://localhost/users/delete"))
            {
                // Verify outcome
                response.Status.Should().Be(JSendStatus.Success);
            }
        }

        [Theory, JSendAutoData]
        public async Task PostsContentToEndpoint(User inputData, JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.PostAsync<User>("http://localhost/users/post-echo", inputData))
            {
                // Verify outcome
                response.Data.ShouldBeEquivalentTo(inputData);
            }
        }

        [Theory, JSendAutoData]
        public async Task PutsContentInEndpoint(User inputData, JSendClient client)
        {
            // Exercise system
            using (client)
            using (var response = await client.PutAsync<User>("http://localhost/users/put-echo", inputData))
            {
                // Verify outcome
                response.Data.ShouldBeEquivalentTo(inputData);
            }
        }
    }
}
