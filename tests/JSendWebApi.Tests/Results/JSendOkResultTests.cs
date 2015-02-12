using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Results;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace JSendWebApi.Tests.Results
{
    public class JSendOkResultTests
    {
        [Fact]
        public void IsHttpActionResult()
        {
            // Fixture setup
            var controller = new Mock<ApiController>().Object;
            // Exercise system
            var result = new JSendOkResult(controller);
            // Verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Fact]
        public async Task ExecuteAsyncReturnsSuccessJSendResponse()
        {
            // Fixture setup
            var controller = new Mock<ApiController>().Object;
            controller.Request = new HttpRequestMessage();

            var jsendSuccess = JsonConvert.SerializeObject(new SuccessJSendResponse());
            var result = new JSendOkResult(controller);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }
    }
}
