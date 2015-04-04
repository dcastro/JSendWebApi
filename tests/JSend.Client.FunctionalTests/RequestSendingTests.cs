using System;
using System.Threading.Tasks;
using FluentAssertions;
using JSend.Client.FunctionalTests.FixtureCustomizations;
using Xunit.Extensions;

namespace JSend.Client.FunctionalTests
{
    public class RequestSendingTests
    {
        [Theory]
        [InlineJSendAutoData("http://localhost/users/get", null)]
        [InlineJSendAutoData("http://localhost/users/get", "")]
        [InlineJSendAutoData("http://localhost/users/", "get")]
        [InlineJSendAutoData(null, "http://localhost/users/get")]
        public async Task SendsGetRequestsToTheCorrectEndpoint(string baseAddress, string requestUri, JSendClient client)
        {
            using (client)
            {
                // Fixture setup
                client.HttpClient.BaseAddress = baseAddress == null ? null : new Uri(baseAddress);

                // Exercise system
                using (var response = await client.GetAsync<string>(requestUri))
                {
                    // Verify outcome
                    response.Status.Should().Be(JSendStatus.Success);
                    response.Data.Should().Be("get");
                }
            }
        }

        [Theory]
        [InlineJSendAutoData("http://localhost/users/post", null)]
        [InlineJSendAutoData("http://localhost/users/post", "")]
        [InlineJSendAutoData("http://localhost/users/", "post")]
        [InlineJSendAutoData(null, "http://localhost/users/post")]
        public async Task SendsPostRequestsToTheCorrectEndpoint(
            string baseAddress, string requestUri,
            User user, JSendClient client)
        {
            using (client)
            {
                // Fixture setup
                client.HttpClient.BaseAddress = baseAddress == null ? null : new Uri(baseAddress);

                // Exercise system
                using (var response = await client.PostAsync<string>(requestUri, user))
                {
                    // Verify outcome
                    response.Status.Should().Be(JSendStatus.Success);
                    response.Data.Should().Be("post");
                }
            }
        }

        [Theory]
        [InlineJSendAutoData("http://localhost/users/put", null)]
        [InlineJSendAutoData("http://localhost/users/put", "")]
        [InlineJSendAutoData("http://localhost/users/", "put")]
        [InlineJSendAutoData(null, "http://localhost/users/put")]
        public async Task SendsPutRequestsToTheCorrectEndpoint(
            string baseAddress, string requestUri,
            User user, JSendClient client)
        {
            using (client)
            {
                // Fixture setup
                client.HttpClient.BaseAddress = baseAddress == null ? null : new Uri(baseAddress);

                // Exercise system
                using (var response = await client.PutAsync<string>(requestUri, user))
                {
                    // Verify outcome
                    response.Status.Should().Be(JSendStatus.Success);
                    response.Data.Should().Be("put");
                }
            }
        }

        [Theory]
        [InlineJSendAutoData("http://localhost/users/delete", null)]
        [InlineJSendAutoData("http://localhost/users/delete", "")]
        [InlineJSendAutoData("http://localhost/users/", "delete")]
        [InlineJSendAutoData(null, "http://localhost/users/delete")]
        public async Task SendsDeleteRequestsToTheCorrectEndpoint(
            string baseAddress, string requestUri,
            JSendClient client)
        {
            using (client)
            {
                // Fixture setup
                client.HttpClient.BaseAddress = baseAddress == null ? null : new Uri(baseAddress);

                // Exercise system
                using (var response = await client.DeleteAsync(requestUri))
                {
                    // Verify outcome
                    response.Status.Should().Be(JSendStatus.Success);
                }
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
