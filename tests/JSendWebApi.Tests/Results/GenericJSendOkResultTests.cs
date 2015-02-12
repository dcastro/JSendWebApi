using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FluentAssertions;
using JSendWebApi.Responses;
using JSendWebApi.Results;
using JSendWebApi.Tests.TestClasses;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace JSendWebApi.Tests.Results
{
    public class GenericJSendOkResultTests
    {
        [Fact]
        public void IsHttpActionResult()
        {
            // Fixture setup
            var controller = new Mock<ApiController>().Object;
            var model = new Model();
            // Exercise system
            var result = new JSendOkResult<Model>(controller, model);
            // Verify outcome
            result.Should().BeAssignableTo<IHttpActionResult>();
        }

        [Fact]
        public async Task ExecuteAsyncReturnsSuccessJSendResponse()
        {
            // Fixture setup
            var controller = new Mock<ApiController>().Object;
            controller.Request = new HttpRequestMessage();
            var model = new Model {Name = "test"};

            var jsendSuccess = JsonConvert.SerializeObject(new SuccessJSendResponse<Model>(model));
            var result = new JSendOkResult<Model>(controller, model);
            // Exercise system
            var message = await result.ExecuteAsync(new CancellationToken());
            // Verify outcome
            var content = await message.Content.ReadAsStringAsync();
            content.Should().Be(jsendSuccess);
        }
    }
}
